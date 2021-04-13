
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
connection.start();
var maxQuantity = 10;

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

connection.on("setMaxQuantity", function (quantity) {
    document.getElementById('quantityvalue').value = "1";
    maxQuantity = parseInt(quantity);
});

function sendMessage() {
    var message = document.getElementById('message-messageinput').value;
    connection.invoke("sendMessage", getSessionID(), message);
}

function setConnectionID() {
    connection.invoke("setConnectionID", getSessionID());
}

function addToBasket() {
    var quantity = document.getElementById('quantityvalue').value;
    var id = document.getElementById('input-size').value;
    var arg = getSessionID() + "," + id + "," + quantity;
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

function updateSmallParcel() {
    var price = document.getElementById('input-cost-small').value;
    connection.invoke("updateParcel", getSessionID(), price, 1);
}

function updateLargeParcel() {
    var price = document.getElementById('input-cost-large').value;
    connection.invoke("updateParcel", getSessionID(), price, 2);
}

function saveShopChanges() {
    var vat = document.getElementById('input-vat').value;
    connection.invoke("saveShopTabChanges", getSessionID(), vat);
    updateSmallParcel();
    updateLargeParcel();
}

function addCategory() {
    var category = document.getElementById('input-category').value;
    connection.invoke("addCategory", getSessionID(), category);
}

function updateEmailTemplate(id) {
    var subject = document.getElementById('input-subject-' + id).value;
    var heading = document.getElementById('input-heading-' + id).value;
    var body = document.getElementById('input-body-' + id).value;
    connection.invoke("UpdateEmailTemplate", getSessionID(), id, subject, heading, body);
}

function deleteCategory(id) {
    connection.invoke("deleteCategory", getSessionID(), id);
    var cat = document.getElementById('category-line-' + id);
    cat.parentNode.removeChild(cat);
}

function updateQuantity() {
    if (window.location.href.toString().includes("/Product")) {
        var stockID = document.getElementById('input-size').value;
        connection.invoke("updateQuantity", stockID);
    }
}
