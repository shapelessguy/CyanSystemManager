using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static CyanSystemManager.Settings;

namespace CyanSystemManager
{
    public class FirebaseClass
    {
        static public bool silent = true;
        static public FirebaseAuthLink token = null;
        static public FirebaseAuthProvider auth;
        static public bool connesso = false;

        static public async Task<bool> CheckConnection()
        {
            //string externalObj;
            try
            {
                WebClient client = new WebClient();
                //client.OpenReadCompleted += (o, e) => externalObj = e.Result.ToString();
                Stream stream = await client.OpenReadTaskAsync(new Uri("http://google.com/generate_204", UriKind.Absolute));
            }
            catch (Exception)
            {
                connesso = false;
                return false;
            }
            connesso = true;
            return true;
        }
        static public void Check_Connection()
        {
            //string externalObj;
            try
            {
                WebClient client = new WebClient();
                //client.OpenReadCompleted += (o, e) => externalObj = e.Result.ToString();
                Stream stream = client.OpenRead(new Uri("http://google.com/generate_204", UriKind.Absolute));
            }
            catch (Exception)
            {
                connesso = false;
                return;
            }
            connesso = true;
        }

        static public async Task<string> GetProvider()
        {
            string firebaseApiKey = Properties.Settings.Default.firebaseApiKey;

            if (!await CheckConnection()) { return "Errore di connessione"; }
            try
            {
                auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(firebaseApiKey));
            }
            catch (Exception) { return "Errore di connessione al server, riprova più tardi"; }
            return "API key accepted";
        }
        static public async Task<string> CheckFirebaseCred(string firebaseUsername = "", string firebasePassword = "")
        {
            string result = await GetProvider();
            if (!silent) Console.WriteLine(result);
            bool tryAgain = true;
            int iteration = 0;
            string error = "Errore";
            do
            {
                iteration++;
                if (iteration > 10) { return "Errore di autenticazione sconosciuto, riprova più tardi"; }
                if (!silent) Console.WriteLine("Trying to authenticate.. iteration: {0}", iteration);
                try
                {
                    if (firebaseUsername == "" && firebasePassword == "") token = await auth.SignInAnonymouslyAsync();
                    else token = await auth.SignInWithEmailAndPasswordAsync(firebaseUsername, firebasePassword);
                    tryAgain = false;
                    return "true";
                }
                catch (FirebaseAuthException faException)
                {
                    if(!silent) Console.WriteLine("Error with authentication: " + faException.Reason.ToString());
                    if (faException.Reason.ToString() == "Undefined") error = "Undefined";
                    else if (faException.Reason.ToString() == "InvalidEmailAddress") error = "E-mail non valida";
                    else if (faException.Reason.ToString() == "UnknownEmailAddress") error = "E-mail non registrata";
                    else if (faException.Reason.ToString() == "WrongPassword" || faException.Reason.ToString() == "Undefined") error = "Password errata";
                    else error = "Errore di autenticazione";

                    tryAgain = false;
                    return error;
                }
                catch (Exception e) { if (!silent) Console.WriteLine("Error while checking FirebaseCreds\n" + e.Message); }
            }
            while (tryAgain);
            return "false";
        }
        static public bool FireBaseLogIn()
        {
            const string firebaseUrl = "https://mobilemoneyguard.firebaseio.com/";
            try
            {
                var firebase = new FirebaseClient(firebaseUrl, new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) });
            }
            catch (Exception ex) { Console.WriteLine("Issue while logging in Firebase\n" + ex); return false; }
            if (!silent && token != null) Console.WriteLine("User Authenticated - " + token.User.Email);
            return true;
        }

        static public async Task FireBaseLogIn(string firebaseUsername, string firebasePassword)
        {
            await CheckFirebaseCred(firebaseUsername, firebasePassword);
            FireBaseLogIn();
            return;
        }
        public static async Task<bool> ChangePassword(string pass)
        {
            try
            {
                await auth.ChangeUserPassword(token.FirebaseToken, pass);
                Console.WriteLine("Password changed into: " + pass + " for the account: " + token.User.Email);
                return true;
            }
            catch (Exception e) { Console.WriteLine("Exception  "+e.Message); return false; }
        }
        public static async Task CreateUser(string firebaseUsername, string firebasePassword)
        {
            if (auth == null) await GetProvider();
            try
            {
                token = await auth.CreateUserWithEmailAndPasswordAsync(firebaseUsername, firebasePassword, "User", false);
            }
            catch (Exception e) { Console.WriteLine(e); Console.WriteLine(token); }
            return;
        }

        static public EventWaitHandle quit = new AutoResetEvent(false);
        public static bool error = false;
        public static string IP = "";
        static public bool uploading = false;
        public static void UploadIP()
        {
            if (uploading) return;
            try
            {
                uploading = true;
                if (!silent) Console.WriteLine("Uploading IP");
                IP = new WebClient().DownloadString("http://icanhazip.com");
                error = false;
                Thread register = new Thread(Register);
                register.Start();
            }
            catch (Exception) { }
            uploading = false;
        }
        private static async void Register()
        {
            if (token == null || (int)((DateTime.Now - token.Created).TotalSeconds) > 30)
            {
                string result = await FirebaseClass.CheckFirebaseCred("firebase_ip_server@gmail.com", "123456");
                if (result == "false") await CreateUser("firebase_ip_server@gmail.com", "123456");
                FireBaseLogIn();
            }
            if (token == null) { error = true; }
            await UploadIP_onStorage();
        }

        public static async Task UploadIP_onStorage()
        {
            if (token == null) { Console.WriteLine("LogIn required."); return; }
            try
            {
                string file_storage = Environment.MachineName + "/IP.txt";
                string file_local = variablePath.networkPath + @"\IP.txt";
                using (var sw = new StreamWriter(file_local)) { if (IP != "") sw.Write(IP); }
                using (var stream = File.Open(file_local, FileMode.Open))
                {
                    FirebaseStorageOptions op = new FirebaseStorageOptions() { AuthTokenAsyncFactory = () => Task.FromResult(token.FirebaseToken) };
                    var task = new FirebaseStorage("ip-manager42.appspot.com", op).Child(file_storage).PutAsync(stream);
                    await task;
                };
                Console.WriteLine("IP has been uploaded -> " + DateTime.Now);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in uploading: " + e.Message);
                error = true;
                return;
            }
            return;
        }

    }
}