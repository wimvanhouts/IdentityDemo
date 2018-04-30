// Log function writes the result of the operation and the status to the html
// window for information
function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

// Attach event handlers to the different buttons of the application
document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);
document.getElementById("logout").addEventListener("click", logout, false);

// Authentication configuration for the openID connect client in he oidc-client.Js file
// we will set basic properties here for the OIDC flow
var config = {
    authority: "http://localhost:54321",                                // --> Address of the identity service
    client_id: "javascript",                                            // --> Javascript client declartion in the identity service, this can be seen as the "type" of application that will connect
    redirect_uri: "http://localhost:55555/callback.html",
    response_type: "id_token token",
    scope: "openid profile demoapi",    // --> Next to the openID connect scopes we must also request access to our API scope
    post_logout_redirect_uri: "http://localhost:55555/index.html",
};
var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {
    if (user) {
        log("User logged in", user.profile);
    }
    else {
        log("User not logged in");
    }
});

function login() {
    mgr.signinRedirect();
}

function api() {
    mgr.getUser().then(function (user) {
        //var url = "http://localhost:54321/identity";
        var url = "http://localhost:12345/api/protected";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function logout() {
    mgr.signoutRedirect();
}