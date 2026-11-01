// Theme colors for charts
const themeColor = name =>
  getComputedStyle(document.documentElement).getPropertyValue(name).trim();

// Get Transactions

const transactions = JSON.parse(
  localStorage.getItem("pennywiseTransactions")
) || [];

// DOM Elements

const reportPeriod = document.getElementById("reportPeriod");
let incomeExpenseChart = null;
let expenseCategoryChart = null;

// Format Currency

function formatCurrency(amount) {
  return new Intl.NumberFormat("en-EG", {
    style: "currency",
    currency: "EGP",
    maximumFractionDigits: 2
  }).format(amount);
}

// Filter Transactions by Period

function getFilteredTransactions() {
  const period = reportPeriod.value;
  const today = new Date();
  return transactions.filter(transaction => {
    const date = new Date(transaction.date + "T00:00:00");
    if (Number.isNaN(date.getTime())) return false;
    if (period === "week") {
      const start = new Date(today);
      start.setDate(today.getDate() - 6);
      start.setHours(0, 0, 0, 0);
      return date >= start && date <= today;
    }
    if (period === "month") {
      return date.getMonth() === today.getMonth() &&
      date.getFullYear() === today.getFullYear();
    }
    if (period === "year") {
      return date.getFullYear() === today.getFullYear();
    }
    return true;
  });
}

// Calculate Summary

function calculateSummary(data) {
  const income = data
    .filter(item => item.type === "income")
    .reduce((sum, item) => sum + Number(item.amount), 0);
  const expenses = data
    .filter(item => item.type === "expense")
    .reduce((sum, item) => sum + Number(item.amount), 0);
  const savings = income - expenses;
  document.getElementById("reportIncome").textContent =
    formatCurrency(income);
  document.getElementById("reportExpenses").textContent =
    formatCurrency(expenses);
  document.getElementById("reportSavings").textContent =
    formatCurrency(savings);
  document.getElementById("reportSavings").style.color =
    savings < 0 ? "#E87979" : "";
  document.getElementById("reportCount").textContent =
    data.length;
  return { income, expenses, savings };
}

// Get Category Totals

function getCategoryTotals(data) {
  const totals = {};
  data
    .filter(item => item.type === "expense")
    .forEach(item => {
      const category = item.category || "Other";
      totals[category] = (totals[category] || 0) +
      Number(item.amount);
    });
  return totals;
}

// Income vs Expenses Chart

function renderIncomeExpenseChart(summary) {
  const canvas = document.getElementById("incomeExpenseChart");
  if (incomeExpenseChart) {
    incomeExpenseChart.destroy();
  }
  incomeExpenseChart = new Chart(canvas, {
    type: "bar",
    data: {
      labels: ["Income", "Expenses"],
      datasets: [{
        label: "Amount (EGP)",
        data: [summary.income, summary.expenses],
        backgroundColor: [
          "#6FB8E6",
          "#E87979"
        ],
      borderRadius: 8,
      barThickness: 55
    }]
  },
  options: {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: false
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
        beginAtZero: true,
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

// Expense Category Chart

function renderCategoryChart(categoryTotals) {
  const canvas = document.getElementById("expenseCategoryChart");
  if (expenseCategoryChart) {
    expenseCategoryChart.destroy();
  }
  const labels = Object.keys(categoryTotals);
  const values = Object.values(categoryTotals);
  const colors = [
    "#F2E199",
    "#6FB8E6",
    "#E87979",
    "#82C9A0",
    "#A8A5D8",
    "#E6A76F",
    "#91B7C9"
  ];
  expenseCategoryChart = new Chart(canvas, {
    type: "doughnut",
    data: {
      labels: labels.length ? labels : ["No Data"],
      datasets: [{
        data: values.length ? values : [1],
        backgroundColor: values.length ? colors.slice(0, values.length) : ["#29415C"],
        borderColor: themeColor("--color-surface"),
        borderWidth: 3,
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
        },
        tooltip: {
          callbacks: {
            label: function(context) {
              if (!values.length) return "No expenses";
              return context.label + ": " +
              formatCurrency(context.raw);
            }
          }
        }
      }
    }
  });
}

// Financial Insights

function renderInsights(summary, categoryTotals) {
  const insightsGrid = document.getElementById("insightsGrid");
  insightsGrid.innerHTML = "";
  const insights = [];
  if (summary.income === 0 && summary.expenses === 0) {
    insightsGrid.innerHTML =
    '<p class="no-insights">Add transactions to discover your financial insights.</p>';
    return;
  }
  if (summary.income > 0) {
    const savingsRate =
      (summary.savings / summary.income) * 100;
    if (savingsRate >= 20) {
      insights.push({
        icon: "piggy-bank",
        title: "Great Saving!",
        text: `You saved ${Math.round(savingsRate)}% of your income. Keep it up!`
      });
    } else if (savingsRate >= 0) {
      insights.push({
        icon: "wallet",
        title: "Saving Opportunity",
        text: "Try to increase the percentage of your income that you save."
      });
    } else {
      insights.push({
        icon: "alert-triangle",
        title: "Spending Alert",
        text: "Your expenses are higher than your income."
      });
    }
  }
  const categories = Object.entries(categoryTotals);
  if (categories.length > 0) {
    const highest = categories.sort((a, b) => b[1] - a[1])[0];
    insights.push({
      icon: "trending-up",
      title: "Top Spending Category",
      text: `${highest[0]} is your highest spending category with ${formatCurrency(highest[1])}.`
    });
  }
  if (summary.expenses === 0) {
    insights.push({
      icon: "check-circle",
      title: "No Expenses",
      text: "You haven't recorded any expenses in this period."
    });
  }
  insights.forEach(insight => {
    const card = document.createElement("article");
    card.className = "insight-card";
    card.innerHTML = `
      <div class="insight-icon">
        <i data-lucide="${insight.icon}"></i>
      </div>
      <div>
        <h3>${insight.title}</h3>
        <p>${insight.text}</p>
      </div>
    `;
    insightsGrid.appendChild(card);
  });
  lucide.createIcons();
}

// Expense Breakdown

function renderBreakdown(categoryTotals) {
  const breakdownList = document.getElementById("breakdownList");
  const breakdownEmpty = document.getElementById("breakdownEmpty");
  breakdownList.innerHTML = "";
  const entries = Object.entries(categoryTotals)
    .sort((a, b) => b[1] - a[1]);
  const totalExpenses = entries.reduce(
    (sum, item) => sum + item[1], 0
  );
  if (entries.length === 0) {
    breakdownEmpty.hidden = false;
    return;
  }
  breakdownEmpty.hidden = true;
  const colors = [
    "#F2E199",
    "#6FB8E6",
    "#E87979",
    "#82C9A0",
    "#A8A5D8",
    "#E6A76F",
    "#91B7C9"
  ];
  entries.forEach(([category, amount], index) => {
    const percentage = (amount / totalExpenses) * 100;
    const row = document.createElement("div");
    row.className = "breakdown-row";
    const dot = document.createElement("span");
    dot.className = "breakdown-dot";
    dot.style.backgroundColor = colors[index % colors.length];
    const categoryElement = document.createElement("div");
    categoryElement.className = "breakdown-category";
    categoryElement.append(dot, document.createTextNode(category));
    const amountElement = document.createElement("strong");
    amountElement.className = "breakdown-amount";
    amountElement.textContent = formatCurrency(amount);
    const percentageElement = document.createElement("span");
    percentageElement.className = "breakdown-percentage";
    percentageElement.textContent = `${percentage.toFixed(1)}%`;
    row.append(categoryElement, amountElement, percentageElement);
    breakdownList.appendChild(row);
  });
}

// Update Report

function updateReport() {
  const data = getFilteredTransactions();
  const summary = calculateSummary(data);
  const categoryTotals = getCategoryTotals(data);
  renderIncomeExpenseChart(summary);
  renderCategoryChart(categoryTotals);
  renderInsights(summary, categoryTotals);
  renderBreakdown(categoryTotals);
  lucide.createIcons();
}

// Export Report

document.getElementById("printReport").addEventListener("click", () => {
  window.print();
});

// Sidebar

const sidebarToggle = document.getElementById("sidebarToggle");
const sidebar = document.getElementById("sidebar");
if (sidebarToggle && sidebar) {
  sidebarToggle.addEventListener("click", () => {
    sidebar.classList.toggle("open");
  });
}

// Initialize

reportPeriod.addEventListener("change", updateReport);
updateReport();
lucide.createIcons();