// Automatically redirects after 5 seconds and counts down
document.addEventListener('DOMContentLoaded', function () {
    var countdownElement = document.getElementById('countdown');
    var countdown = 3; // seconds

    function updateCountdown() {
        countdownElement.textContent = countdown;

        if (countdown === 0) {
            window.location.href = 'https://localhost:7244/';
        } else {
            countdown--;
        }
    }

    // Update every second
    setInterval(updateCountdown, 1000);
    // Run for the first time to display immediately
    updateCountdown();
});
