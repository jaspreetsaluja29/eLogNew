function togglePassword() {
    const passwordField = document.getElementById('password');
    if (passwordField.type === 'password') {
        passwordField.type = 'text';
    } else {
        passwordField.type = 'password';
    }
}

// Add event listeners when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    // Add click event listener to the password toggle button
    const toggleBtn = document.querySelector('.toggle-password');
    if (toggleBtn) {
        toggleBtn.addEventListener('click', togglePassword);
    }

    // Form submission handling
    const loginForm = document.querySelector('#login-form');
    if (loginForm) {
        // We're removing the preventDefault() call to allow the form to submit naturally
        // This way the browser will handle the form submission to the server
        // You can still do client-side validation if needed
        loginForm.addEventListener('submit', function (event) {
            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;
            if (!username || !password) {
                alert('Please enter both Username and Password');
                event.preventDefault(); // Only prevent submission if validation fails
                return false;
            }
            // If validation passes, let the form submit naturally to the controller
            return true;
        });
    }

    // Reset button handling
    const resetButton = document.getElementById('reset-button');
    if (resetButton) {
        resetButton.addEventListener('click', function (e) {
            e.preventDefault(); // Prevent the default form reset behavior
            // Clear the form fields manually
            document.getElementById('username').value = '';
            document.getElementById('password').value = '';
            // Clear any error messages if they exist
            const errorMessage = document.getElementById('error-message');
            if (errorMessage) {
                errorMessage.style.display = 'none';
            }
        });
    }

    // Check if OTP section needs to be displayed (for when success message exists)
    const otpSection = document.getElementById('otp-section');
    if (otpSection && document.querySelector('.success-message')) {
        otpSection.style.display = 'block';
    }
});