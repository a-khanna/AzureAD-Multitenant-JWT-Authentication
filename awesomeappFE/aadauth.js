const CLIENTID = 'f0d664cd-09bf-42b0-9b96-fdc9cbc77ebf';

var clientApplication = new Msal.UserAgentApplication(CLIENTID, null, authCallback);

var username = $('#user_name');
var loginbtn = $('#login_button');
var logoutbtn = $('#logout_button');
var notregistered = $('#not_registered');
notregistered.hide();

var signOutIfTenantNotFound = user => {
	let tenants = sessionStorage.getItem('tenants');
	let tenant = user.idToken.tid;
	if (!tenants || !tenants.includes(tenant)) {	
		clientApplication.logout();
		notregistered.show();
	}
};

var getData = (url, callback, tokenrefresh = false) => {
	let token = sessionStorage.getItem('msal.idtoken');
	let statuscode;
	return fetch(url, {
		method: 'GET',
		headers: {
			'Authorization': 'Bearer ' + token
		}
	})
	.then(res => {
		statuscode = res.status;
		return res.json();
	})
	.then(callback)
	.catch(err => {
		if (statuscode == 401 && !tokenrefresh) {
			console.log('Refreshing token!');
			clientApplication.acquireTokenSilent([CLIENTID])
				.then(_ => {
					console.log('silenttoken', _);
					getData(url, callback, true);
				}, error => {
					console.log(error);
					clientApplication.acquireTokenPopup([CLIENTID])
						.then(_ => getData(url, callback, true), error => console.log(error));
				});
		}
		console.log(err);
	})
};

var onSignin = idToken => {
    var user = clientApplication.getUser();
    if (user) {
		signOutIfTenantNotFound(user);
		username.html(user.displayableId);
		loginbtn.hide();
		logoutbtn.show();
		getData('https://localhost:44335/api/notifications', data => console.log(data));
    } else {
        username.empty();
        loginbtn.show();
        logoutbtn.hide();
    }
};

onSignin(null);

loginbtn.click(() => {
    clientApplication.loginPopup().then(onSignin);
});

logoutbtn.click(() => {
    clientApplication.logout();
});

var authCallback = (errorDesc, token, error, tokenType) => {
    //This function is called after loginRedirect and acquireTokenRedirect. Not called with loginPopup
    // msal object is bound to the window object after the constructor is called.
    console.log('authCallBack called!');
    if (token) {
        console.log(token);
    }
    else {
        console.log(error + ":" + errorDesc);
    }
}


getData('https://localhost:44335/api/auth/tenants', data => sessionStorage.setItem('tenants', data));