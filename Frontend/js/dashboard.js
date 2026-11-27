// Theme colors for charts
const themeColor = name =>
  getComputedStyle(document.documentElement).getPropertyValue(name).trim();

// Lucide Icons
lucide.createIcons();

// Sidebar Toggle
const sidebar = document.getElementById("sidebar");
const sidebarToggle = document.getElementById("sidebarToggle");
if (sidebar && sidebarToggle) {
  sidebarToggle.addEventListener("click", function () {
    sidebar.classList.toggle("open");
  });
}

// Add Transaction Button
const addTransactionBtn = document.getElementById("addTransactionBtn");
if (addTransactionBtn) {
    addTransactionBtn.addEventListener("click", function () {
        window.location.href = "transactions.html?add=true";
    });
}

// Income vs Expenses Chart
const spendingCanvas = document.getElementById("spendingChart");
if (spendingCanvas) {
  const spendingChart = new Chart(spendingCanvas, {
    type: "bar",
    data: {
      labels: ["Week 1", "Week 2", "Week 3", "Week 4"],
      datasets: [
        {
          label: "Income",
          data: [4000, 5000, 3500, 6000],
          backgroundColor: "#6FB8E6",
          borderRadius: 6
        },
        {
          label: "Expenses",
          data: [1500, 2200, 1800, 2500],
          backgroundColor: "#F2E199",
          borderRadius: 6
        }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          labels: {
            color: themeColor("--color-text"),
            usePointStyle: true
          }
        }
      },
      scales: {
        x: {
          ticks: {
            color: themeColor("--color-text-muted")
          },
          grid: {
            display: false
          }
        },
        y: {
          ticks: {
            color: themeColor("--color-text-muted")
          },
          grid: {
            color: themeColor("--color-border")
          }
        }
      }
    }
  });
}

// Expense Categories Chart
const categoryCanvas = document.getElementById("categoryChart");
if (categoryCanvas) {
  const categoryChart = new Chart(categoryCanvas, {
    type: "doughnut",
    data: {
      labels: [
        "Food",
        "Shopping",
        "Transport",
        "Entertainment"
      ],
      datasets: [{
        data: [35, 30, 20, 15],
        backgroundColor: [
          "#6FB8E6",
          "#F2E199",
          "#69D6A3",
          "#E6A76C"
        ],
        borderColor: themeColor("--color-surface"),
        borderWidth: 4,
        hoverOffset: 8
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: "bottom",
          labels: {
            color: themeColor("--color-text"),
            padding: 18,
            usePointStyle: true
          }
        }
      }
    }
  });
}