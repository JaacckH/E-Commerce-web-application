
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
connection.start();

setTimeout(test, 1000);

function test() {
    connection.invoke("test", "text");
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
