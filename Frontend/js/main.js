// Mobile Navigation
const menuToggle = document.getElementById("menuToggle");
const navLinks = document.getElementById("navLinks");

// Toggle Menu
menuToggle.addEventListener("click", function() {
  navLinks.classList.toggle("open");
  const isOpen = navLinks.classList.contains("open");
  menuToggle.setAttribute("aria-expanded", isOpen);
  menuToggle.setAttribute("aria-label", isOpen? "Close navigation menu" : "Open navigation menu");
});

// Close menu when clicking a link
const navItems = document.querySelectorAll(".nav-link");
navItems.forEach(function (link) {
  link.addEventListener("click", function() {
    navLinks.classList.remove("open");
    menuToggle.setAttribute("aria-expanded", "false");
    menuToggle.setAttribute("aria-label", "Open navigation menu");
  });
});

// Close menu when pressing escape
document.addEventListener("keydown", function(event) {
  if(event.key == "Escape") {
    navLinks.classList.remove("open");
    menuToggle.setAttribute("aria-expanded", "false");
    menuToggle.setAttribute("aria-label","Open navigation menu");
  }
})