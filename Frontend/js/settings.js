// Settings Elements

const settingsForm = document.getElementById("settingsForm");
const settingsMessage = document.getElementById("settingsMessage");
const fullName = document.getElementById("fullName");
const email = document.getElementById("email");
const currency = document.getElementById("currency");
const language = document.getElementById("language");
const theme = document.getElementById("theme");
const budgetNotifications = document.getElementById("budgetNotifications");
const transactionNotifications = document.getElementById("transactionNotifications");
const reportNotifications = document.getElementById("reportNotifications");
const currentPassword = document.getElementById("currentPassword");
const newPassword = document.getElementById("newPassword");
const confirmPassword = document.getElementById("confirmPassword");

// Default Settings

const defaultSettings = {
  fullName: "Ghofran Mohamed",
  email: "ghofran@example.com",
  currency: "EGP",
  language: "English",
  theme: "dark",
  budgetNotifications: true,
  transactionNotifications: true,
  reportNotifications: false
};

// Load Saved Settings

function loadSettings() {
  const savedSettings = JSON.parse(
    localStorage.getItem("pennywiseSettings")
  ) || defaultSettings;
  fullName.value = savedSettings.fullName;
  email.value = savedSettings.email;
  currency.value = savedSettings.currency;
  language.value = savedSettings.language;
  theme.value = savedSettings.theme;
  budgetNotifications.checked = savedSettings.budgetNotifications;
  transactionNotifications.checked = savedSettings.transactionNotifications;
  reportNotifications.checked = savedSettings.reportNotifications;
  applyTheme(savedSettings.theme);
}

// Save Settings

settingsForm.addEventListener("submit", function(event) {
  event.preventDefault();
  // Validate Password Fields
  if (
    currentPassword.value ||
    newPassword.value ||
    confirmPassword.value
  ) {
  if (!currentPassword.value || !newPassword.value || !confirmPassword.value) {
    showMessage("Please complete all password fields.", "error");
    return;
  }
  if (newPassword.value !== confirmPassword.value) {
    showMessage("New passwords do not match.", "error");
    return;
  }
  if (newPassword.value.length < 8) {
    showMessage("Password must be at least 8 characters.", "error");
    return;
  }
  showMessage(
    "Password changes require backend authentication.",
    "error"
  );
  return;
}
// Create Settings Object
const settings = {
  fullName: fullName.value.trim(),
  email: email.value.trim(),
  currency: currency.value,
  language: language.value,
  theme: theme.value,
  budgetNotifications: budgetNotifications.checked,
  transactionNotifications: transactionNotifications.checked,
  reportNotifications: reportNotifications.checked
};
// Validate Profile Information
if (!settings.fullName || !settings.email) {
  showMessage("Please fill in all required fields.", "error");
  return;
}
// Save to Local Storage
localStorage.setItem(
  "pennywiseSettings",
  JSON.stringify(settings)
);
applyTheme(settings.theme);
showMessage("Settings saved successfully!", "success");
});

// Reset Settings

settingsForm.addEventListener("reset", function() {
  setTimeout(function() {
    localStorage.removeItem("pennywiseSettings");
    loadSettings();
    currentPassword.value = "";
    newPassword.value = "";
    confirmPassword.value = "";
    showMessage("Settings restored to default.", "success");
  }, 0);
});

// Show Message

function showMessage(message, type) {
  settingsMessage.textContent = message;
  settingsMessage.style.color =
    type === "success"
    ? "var(--color-secondary)"
    : "#ff8585";
}

// Preview theme as soon as it is changed
theme.addEventListener("change", function() {
  applyTheme(theme.value);
});

// Initialize

loadSettings();
if (window.lucide) {
  lucide.createIcons();
}

// Sidebar Toggle

const menuToggle = document.getElementById("menuToggle");
const sidebar = document.getElementById("sidebar");
if (menuToggle && sidebar) {
  menuToggle.addEventListener("click", function() {
    sidebar.classList.toggle("open");
  });
}