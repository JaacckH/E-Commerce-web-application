
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
connection.start();

setTimeout(300, setConnectionID);

connection.on("setSessionID", function (sessionID) {
    document.cookie = "SessionID=" + sessionID;
});

connection.on("Redirect", function (path) {
    window.location.href = path;
});

connection.on("sendAlert", function (message) {
    alert(message);
});

connection.on("sendSuccessAlert", function (message) {
    alert(message);
});

connection.on("ContentDelivery", function (content, div) {
    document.getElementById(div).innerHTML = content;
});

connection.on("AppendDelivery", function (content, div) {
    document.getElementById(div).innerHTML += content;
});

connection.on("removeContainer", function (id) {
    document.getElementById('basket-product-' + id).outerHTML = "";
});

function sendMessage() {
    var message = document.getElementById('message-messageinput').value;
    connection.invoke("sendMessage", getSessionID(), message);
}

function setConnectionID() {
    connection.invoke("setConnectionID", getSessionID());
}

function addToBasket(id) {
    var quantity = document.getElementById('product-quantity-select-' + id).value;
    var sessionID = getSessionID();
    var arg = sessionID + "," + id + "," + quantity;
    connection.invoke("addToBasket", arg);
}

function removeFromBasket(id) {
    connection.invoke("removeFromBasket", getSessionID(), id);
}

function getSessionID() {
    var name = "SessionID=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length).toString();
        }
    }
    return "";
}

function createAccount() {
    var forename = document.getElementById('forename').value;
    var surname = document.getElementById('surname').value;
    var email = document.getElementById('email').value;
    var password = document.getElementById('password').value;
    var confirmpassword = document.getElementById('confirmpassword').value;
    var addressline1 = document.getElementById('addressline1').value;
    var addressline2 = document.getElementById('addressline2').value;
    var postcode = document.getElementById('postcode').value;
    var phonenumber = document.getElementById('phonenumber').value;

    connection.invoke("createUserAccount", forename, surname, email, password, confirmpassword, addressline1, addressline2, postcode, phonenumber);
}

function login() {
    var email = document.getElementById("useremail").value;
    var password = document.getElementById("userpassword").value;
    connection.invoke("loginUser", email, password);
}

function emailPasswordReset() {
    var email = document.getElementById('resetemail').value;
    connection.invoke("sendEmail", email);
}
function loadProfile() {

}

function updateProfile() {
    var forename = document.getElementById('forename').value;
    var surname = document.getElementById('surname').value;
    var addressline1 = document.getElementById('addressline1').value;
    var addressline2 = document.getElementById('addressline2').value;
    var postcode = document.getElementById('postcode').value;
    var phonenumber = document.getElementById('phonenumber').value;

}

function updateSettings() {
    var email = document.getElementById('inputEmail4').value;
    var addressLine1 = document.getElementById('inputAddress').value;
    var addressLine2 = document.getElementById('inputAddress2').value;
    var zip = document.getElementById('inputZip').value;
    connection.invoke("UpdateSettings", getSessionID(), email, addressLine1, addressLine2, zip);
}
