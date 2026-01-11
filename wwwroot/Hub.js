function setupSignalRConnection() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7269/transactionHub")
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