function setupSignalRConnection() {
    // Use the current origin so the hub uses the same host and port as the app
    const hubUrl = `${window.location.origin}/transactionHub`;

    connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl)
        .withAutomaticReconnect()
        .build();

    connection.on("TransactionUpdated", (order) => {
        if (currentOrderId && order.id === currentOrderId) {
            updateOrderDetails(order);
        }
    });

    connection
        .start()
        .then(() => console.log("Connected to SignalR Hub"))
        .catch((err) => console.error("SignalR connection error", err));
}

//para gumana
setupSignalRConnection();

function updateOrderDetails(order) {
    console.log(order);
}