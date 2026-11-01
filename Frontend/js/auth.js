// Login Form
const loginForm = document.getElementById("loginForm");
if (loginForm) {
  loginForm.addEventListener("submit", function (event) {
    event.preventDefault();
    const email = document.getElementById("loginEmail").value.trim();
    const password = document.getElementById("loginPassword").value;
    const message = document.getElementById("loginMessage");
    if (email === "" || password === "") {
      message.textContent = "Please fill in all fields.";
      message.style.color = "#ff7777";
      return;
    }
    message.textContent = "Login form is ready for backend integration.";
    message.style.color = "#6FB8E6";
  });
}

// Register Form
const registerForm = document.getElementById("registerForm");
if (registerForm) {
  registerForm.addEventListener("submit", function (event) {
    event.preventDefault();
    const name = document.getElementById("registerName").value.trim();
    const email = document.getElementById("registerEmail").value.trim();
    const password = document.getElementById("registerPassword").value;
    const confirmPassword = document.getElementById("confirmPassword").value;
    const message = document.getElementById("registerMessage");
    if (password.length < 8) {
      message.textContent = "Password must be at least 8 characters.";
      message.style.color = "#ff7777";
      return;
    }
    if (password !== confirmPassword) {
      message.textContent = "Passwords do not match.";
      message.style.color = "#ff7777";
      return;
    }
    message.textContent = "Registration form is ready for backend integration.";
    message.style.color = "#6FB8E6";
  });
}