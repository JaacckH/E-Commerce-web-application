
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
connection.start();

setTimeout(test, 1000);

function test() {
    connection.invoke("test", "text");
}
