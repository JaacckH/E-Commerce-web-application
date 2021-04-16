
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
connection.start();
var maxQuantity = 10;
var recipient = "null";

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
    scrollMessages();
});

connection.on("updateBasket", function (content) {
    document.getElementById('basket-counter').innerHTML = '<i class="bi bi - bag - fill"></i> ' + content;
});

connection.on("AppendDelivery", function (content, div) {
    document.getElementById(div).innerHTML += content;
    scrollMessages();
});

connection.on("removeContainer", function (id) {
    document.getElementById('basket-product-' + id).outerHTML = "";
});

connection.on("setMaxQuantity", function (quantity) {
    document.getElementById('quantityvalue').value = "1";
    maxQuantity = parseInt(quantity);
});

connection.on("setCheckoutCookie", function () {
    document.cookie = "rtc=" + "True";
});


function scrollMessages() {
    var chatboxes = document.getElementsByClassName("scroll");
    for (var i = 0; i < chatboxes.length; i++) {
        chatboxes[i].scrollTop = chatboxes[i].scrollHeight;
    }
}

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

    //TEMPORARY
    window.location.reload();
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

    var rtc = false;
    if (getUrlVariable('rtc') == "True") {
        rtc = true;
    }

    connection.invoke("createUserAccount", rtc, getSessionID(), forename, surname, email, password, confirmpassword, addressline1, addressline2, postcode, phonenumber);
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
    var shopTags = document.getElementById('admin-product-tags').value;
    connection.invoke("saveShopTabChanges", getSessionID(), vat, shopTags);
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

function adminSendMessage() {
    var message = document.getElementById('message-messageinput').value;
    connection.invoke("adminSendMessage", getSessionID(), recipient, message)
}

function openChat() {
    hideChat();
    ToggleChatButton();
    setConnectionID();
    connection.invoke("openChat", getSessionID(), recipient);
    scrollMessages();

    setTimeout(function () { 
    if ($('#chatbox').length > 0) {
        console.log('chatbox-exists');
        $("#chatbox").draggable({ handle: "#chatbox-header" });

        }
    }, 300);
}

function hideChat() {
   
    document.getElementById('chatbox-placeholder').innerHTML = "";
    
}

function ToggleChatButton() {
    const element = document.querySelector("#comm-btn");
    element.classList.toggle("d-none");
}

function markAsSettled(user) {
    connection.invoke("markAsSettled", getSessionID(), user);
    window.location.reload();
}


function SendConfirmEmail() {
    var Email = document.getElementById('UserEmail').value;
    
    connection.invoke("confirmOrderEmail", getSessionID(), Email);
}

function getUrlVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split("?");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == variable) { return pair[1]; }
    }
    return (false);
}

function addPromoCode() {
    document.getElementById('promoCode').value;
}