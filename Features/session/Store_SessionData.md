
=============================================================
# what to store in session
* A session should be the minimal amount of information needed to authenticate a user, to link a record in your database ("user") to the client program sending requests
* we may only need to store a uuid as user_id in session;
* Fetching a single user record on each request doesn't seem like a lot of overhead, and you don't have to worry about updates not being synced -> there's middleware attaching an up-to-date user model to each request
* the middleware'll fetches "base" information from what we have in session; something like `select * from vw_user_details where id = $1` and we take the results and slop them into res.locals, and req.user. 
* If that query returns no rows, oops!, the user is gone, turf the session & bump out to login page

* after user login, to  saving queries results and heavy-to-compute you should really use Cache instead
* -> Run your query once, then store the result in the cache, using an identifier that create from the current user's user id as the cache key

* redis is an option, it can be used both for session storage and caching and it also persists to disk so if you redis server crashes you can get all the data back including the sessions which is amazing. It's also super fast just like memcached

## Where to store ?
* Session data can be stored on the client side (but less secure), Ex: in cookies
* stored on the server-side, such as in cache or database

==============================================================
# Implement storing "session data" on "database"

## create a table
* -> the id is the session id, and it's used as the primary key
* -> the expire_at column to indicate the timestamp at which we need to expire the session
* -> data column to hold all the session data
```sql
CREATE TABLE session (
    id varchar(128),
    expire_at int unsigned,
    data longtext
);
```

## Client and Server interactions 
* **Note**: we'll **`tracking a user's behavior even before they log in`** 
* -> For example: in an e-commerce site, we will need to **store the items in a user's shopping cart** even **`before they sign up and log in`**
* -> we create a new session **`for all request`** that **`doesn't contain the session id`** (_Ex: the first request from a user_)
* -> all the subsequent requests will contain a session id 

* _process_:
* -> Client send first request
* -> Server find no Session ID in cookie header; so Server generate a new Session ID
* -> Server create a new session record in Database and send Session ID back to the Client
* -> Client store Session ID in cookie; all subsequent requests contain session id
* -> Server sees a Session ID in the cookie and retrieve session data from database

## Implememnt
* _https://medium.com/webarchitects/implement-user-session-8ce54ebc1cfa_
* -> Note: this is just a a simplified version of session management implementation
* -> In practical, we will need to implement a lot more
* -> for example, if we need to check if the request with the session id comes from the same IP address, you will need to add an IP address column to the session table
* -> we might also need to lock a session while processing a request to prevent race conditions of two requests updating the same session

```cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

class Session
{
    protected SqlConnection pdo;
    protected string session_cookie_name = "session"; // The session cookie name
    protected int session_expiration = 7200; // The number of seconds that a session to last
    protected bool new_session_generated = false;
    protected int cleanup_percentage = 1; // so 1 out of 100 request, we'll call "clean_session"
    
    public Session(string session_cookie_name, int session_expiration, int cleanup_percentage)
    {
        this.session_cookie_name = session_cookie_name;
        this.session_expiration = session_expiration;
        this.cleanup_percentage = cleanup_percentage;
    }
    
    /// <summary>
    /// is called for each request
    /// In an MVC framework, we typically call it in a base controller class before the request is assigned to a controller method
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public void start_session()
    {
        // Connect to the database
        string connectionString = "Data Source=localhost;Initial Catalog=test;User ID=username;Password=password";
        this.pdo = new SqlConnection(connectionString);
        this.pdo.Open();
        
        // use "cleanup_percentage" to control how often we want to clean up "expired sessions"
        if (new Random().Next(1, 100) <= this.cleanup_percentage)
        {
            this.clean_session(); // clean up "expired session data"
        }
        
        string session_id = HttpContext.Current.Request.Cookies[this.session_cookie_name]?.Value;
        Dictionary<string, object> session = null;
        
        if (!string.IsNullOrEmpty(session_id)) 
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM session WHERE id = @session_id", this.pdo);
            cmd.Parameters.AddWithValue("@session_id", session_id);
            SqlDataReader reader = cmd.ExecuteReader();
            
            if (reader.Read())
            {
                session = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    object columnValue = reader.GetValue(i);
                    session[columnName] = columnValue;
                }
            }
            
            reader.Close();
        }
        
        // check if "session id" is not in the "cookie" or not found in the "database"
        if (session == null || string.IsNullOrEmpty(session_id))
        { // -> if not valid, create a new session id
            string new_session_id = Guid.NewGuid().ToString();
            this.new_session_generated = true;

            // set the session id of the current session to the new value
            HttpContext.Current.Session.SessionID = new_session_id; 
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(this.session_cookie_name, new_session_id)
            {
                Expires = DateTime.Now.AddSeconds(this.session_expiration)
            });
        }
        else
        { // -> if session id is valid, load the "session data" from the database into the $_SESSION superglobals
            HttpContext.Current.Session.SessionID = session_id;
            HttpContext.Current.Session.Clear();
            foreach (KeyValuePair<string, object> kvp in session)
            {
                HttpContext.Current.Session[kvp.Key] = kvp.Value;
            }
        }
    }
    
    /// <summary>
    /// will either "insert a new session record" or "update an existing session record"
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public void save_session()
    {
        string data = JsonSerializer.Serialize(HttpContext.Current.Session);
        
        if (this.new_session_generated)
        { // If it's a new record,
            // it will also calculate the "expire_at" value based on the "session_expiration" attribute
            SqlCommand cmd = new SqlCommand("INSERT INTO session (id, expire_at, data) VALUES (@session_id, @expire_at, @data)", this.pdo);
            cmd.Parameters.AddWithValue("@session_id", HttpContext.Current.Session.SessionID);
            cmd.Parameters.AddWithValue("@expire_at", DateTime.Now.AddSeconds(this.session_expiration));
            cmd.Parameters.AddWithValue("@data", data);
            cmd.ExecuteNonQuery();
        }
        else
        {
            SqlCommand cmd = new SqlCommand("UPDATE session SET data = @data WHERE id = @session_id", this.pdo);
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@session_id", HttpContext.Current.Session.SessionID);
            cmd.ExecuteNonQuery();
        }
    }
    
    /// <summary>
    /// deletes a specific session in the database and also removes the session from the browser cookie
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public void destroy_session(string session_id)
    {
        SqlCommand cmd = new SqlCommand("DELETE FROM session WHERE id = @session_id", this.pdo);
        cmd.Parameters.AddWithValue("@session_id", session_id);
        cmd.ExecuteNonQuery();
        
        HttpContext.Current.Response.Cookies.Add(new HttpCookie(this.session_cookie_name, "")
        {
            Expires = DateTime.Now.AddDays(-1)
        });
    }
    
    // Clean expired session
    private void clean_session()
    {
        SqlCommand cmd = new SqlCommand("DELETE FROM session WHERE expired_at <= @current_time", this.pdo);
        cmd.Parameters.AddWithValue("@current_time", DateTime.Now);
        cmd.ExecuteNonQuery();
    }
}
```

* **`how session data is manipulated when a user logs in and out`**
```cs
using System;

class Authentication
{
    protected Session session;
    
    public Authentication(Session session)
    {
        this.session = session;
    }
    
    public void Login()
    {
        // ... Code that validates username and password are omitted

        // If authentication succeeds: 
        string newSessionId = Guid.NewGuid().ToString(); // generate a new session id to prevent session fixation attacks
        string oldSessionId = session.GetSessionId();
        session.SetSessionId(newSessionId);

        session.SetSessionValue("is_logged_in", true); // indicate a user is logged in
        session.SetSessionValue("user_id", GetCurrentUserId()); // store the "user id" in the session to easily get the user data of the current session

        this.pdo.Prepare("UPDATE session SET data = ?, id = ? WHERE id = ?");
        this.pdo.Execute(new object[] {JsonConvert.SerializeObject(session.GetSessionData()), newSessionId, oldSessionId});
    }
    
    public void Logout() // just delegates to the "Session" class's "destroy_session()" method
    {
        session.DestroySession(session.GetSessionId());
    }
}
```