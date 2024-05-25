function checkBalance() {
    var balance = parseInt(document.getElementById("Balance").value);
    // Get the value of the selected radio button
    var selectedOption = document.querySelector('input[name="options"]:checked').value;
    // Separate the number from the value of the radio button
    var price = parseInt(selectedOption.split('-')[1]);

    if (balance < price) {
        toastNotification({
            title: "Thất bại!",
            message: "Số dư của bạn không đủ để thực hiện giao dịch này.",
            type: "error",
            duration: 5000
        });

        // Prevent the form from being sent
        return false; 
    }

    // Allow form to be sent
    return true;
}