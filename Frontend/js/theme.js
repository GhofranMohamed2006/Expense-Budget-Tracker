// Applies the saved theme before the page is drawn (load this in <head>)
(function () {
  function getSavedTheme() {
    try {
      const saved = JSON.parse(localStorage.getItem("pennywiseSettings"));
      return saved && saved.theme === "light" ? "light" : "dark";
    } catch (error) {
      return "dark";
    }
  }

  window.applyTheme = function (theme) {
    document.documentElement.setAttribute(
      "data-theme",
      theme === "light" ? "light" : "dark"
    );
  };

  window.applyTheme(getSavedTheme());
})();