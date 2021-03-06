"use strict"

const pricingConnection = 
    new signalR
        .HubConnectionBuilder()
        .withUrl("/subscribe/infoprice")
        .build();

const showMessage = (content) => {
    var li = document.createElement("li");
    li.textContent = content;
    document.getElementById("messagesList").prepend(li);
};

const setButtonEnabled = status =>
    document.getElementById("startStreaming").disabled = !status;

pricingConnection.start().then( () => {
    setButtonEnabled(true);
    showMessage("Connected with server");
}).catch((err) => {
    showMessage("Failed to connect to server" + err.toString());
});

document
    .getElementById("startStreaming")
    .addEventListener("click", () => {
        setButtonEnabled(false);
        const uic = document.getElementById("uic").value;
        const assestType = document.getElementById("assetType").value;

        pricingConnection.stream("Subscribe", uic, assestType)
            .subscribe({
                next: showMessage,
                complete: () => {
                    showMessage("Stream completed");
                    setButtonEnabled(true);
                },
                error: showMessage,
            });
        event.preventDefault();
    });