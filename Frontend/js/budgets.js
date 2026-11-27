// Sample Budget Data

const defaultBudgets = [
  {
    id: 1,
    name: "Monthly Food",
    category: "Food",
    limit: 5000,
    spent: 3200,
    period: "Monthly"
  },
  {
    id: 2,
    name: "Shopping Budget",
    category: "Shopping",
    limit: 3000,
    spent: 2500,
    period: "Monthly"
  },
  {
    id: 3,
    name: "Transportation",
    category: "Transportation",
    limit: 1500,
    spent: 900,
    period: "Monthly"
  },
  {
    id: 4,
    name: "Entertainment",
    category: "Entertainment",
    limit: 1000,
    spent: 1100,
    period: "Monthly"
  }
];
let budgets = JSON.parse(localStorage.getItem("pennywiseBudgets")) || defaultBudgets;
let editingBudgetId = null;

// DOM Elements

const budgetGrid = document.getElementById("budgetGrid");
const budgetModal = document.getElementById("budgetModal");
const budgetForm = document.getElementById("budgetForm");
const openBudgetModal = document.getElementById("openBudgetModal");
const closeBudgetModal = document.getElementById("closeBudgetModal");
const cancelBudgetModal = document.getElementById("cancelBudgetModal");
const emptyCreateBtn = document.getElementById("emptyCreateBtn");
const budgetFilter = document.getElementById("budgetFilter");

// Save Data

function saveBudgets() {
  localStorage.setItem("pennywiseBudgets", JSON.stringify(budgets));
}

// Format Currency

function formatCurrency(amount) {
  return new Intl.NumberFormat("en-EG", {
    style: "currency",
    currency: "EGP",
    maximumFractionDigits: 2
  }).format(amount);
}

// 5. Get Budget Status

function getBudgetStatus(budget) {
  const percentage = (budget.spent / budget.limit) * 100;
  if (percentage >= 100) {
    return {
      percentage,
      status: "over",
      label: "Over Budget",
      progressClass: "progress-over",
      statusClass: "status-over"
    };
  }
  if (percentage >= 80) {
    return {
      percentage,
      status: "warning",
      label: "Near Limit",
      progressClass: "progress-warning",
      statusClass: "status-warning"
    };
  }
  return {
    percentage,
    status: "safe",
    label: "On Track",
    progressClass: "progress-safe",
    statusClass: "status-safe"
  };
}

// Update Summary Cards

function updateSummary() {
  const totalBudget = budgets.reduce(
    (sum, budget) => sum + budget.limit, 0
  );
  const totalSpent = budgets.reduce(
    (sum, budget) => sum + budget.spent, 0
  );
  const remaining = totalBudget - totalSpent;
  document.getElementById("totalBudget").textContent =
    formatCurrency(totalBudget);
  document.getElementById("totalSpent").textContent =
    formatCurrency(totalSpent);
  document.getElementById("totalRemaining").textContent =
    formatCurrency(remaining);
  document.getElementById("totalRemaining").style.color =
    remaining < 0 ? "#E87979" : "";
  document.getElementById("budgetCount").textContent =
    budgets.length;
}

// Display Budget Cards

function displayBudgets(data = budgets) {
  budgetGrid.innerHTML = "";
  const emptyState = document.getElementById("budgetEmptyState");
  if (data.length === 0) {
    emptyState.hidden = false;
    lucide.createIcons();
    return;
  }
  emptyState.hidden = true;
  data.forEach(function (budget) {
    const result = getBudgetStatus(budget);
    const remaining = budget.limit - budget.spent;
    // Prevent progress bar from exceeding 100%
    const progressWidth = Math.min(result.percentage, 100);
    const card = document.createElement("article");
    card.className = "budget-card";
    card.innerHTML = `
      <div class="budget-card-header">
        <div class="budget-info">
          <div class="budget-icon">
            <i data-lucide="wallet"></i>
          </div>
          <div>
            <h3>${escapeHTML(budget.name)}</h3>
            <p>${escapeHTML(budget.category)} • ${escapeHTML(budget.period)}</p>
          </div>
        </div>
        <div class="budget-actions">
          <button class="edit-budget" data-id="${budget.id}" aria-label="Edit budget">
            <i data-lucide="pencil"></i>
          </button>
          <button class="delete-budget" data-id="${budget.id}" aria-label="Delete budget">
            <i data-lucide="trash-2"></i>
          </button>
        </div>
      </div>
      <div class="budget-amounts">
        <strong>${formatCurrency(budget.spent)}</strong>
        <span>of ${formatCurrency(budget.limit)}</span>
      </div>
      <div class="progress-track">
        <div
          class="progress-fill ${result.progressClass}"
          style="width: ${progressWidth}%"
        ></div>
      </div>
      <div class="budget-footer">
        <span class="budget-status ${result.statusClass}">
          ${result.label} · ${Math.round(result.percentage)}%
        </span>
        <span class="budget-remaining">
          ${remaining >= 0
          ? formatCurrency(remaining) + " left"
          : formatCurrency(Math.abs(remaining)) + " over"}
        </span>
      </div>
    `;
    budgetGrid.appendChild(card);
  });
  lucide.createIcons();
}

// Filter Budgets

function filterBudgets() {
  const selectedStatus = budgetFilter.value;
  if (selectedStatus === "all") {
    displayBudgets(budgets);
    return;
  }
  const filteredBudgets = budgets.filter(function (budget) {
    return getBudgetStatus(budget).status === selectedStatus;
  });
  displayBudgets(filteredBudgets);
}

// Open and Close Modal

function openModal() {
  budgetForm.reset();
  editingBudgetId = null;
  document.getElementById("modalTitle").textContent =
    "Create New Budget";
  document.getElementById("saveBudgetBtn").textContent =
    "Save Budget";
  budgetModal.classList.add("show");
}

function closeModal() {
  budgetModal.classList.remove("show");
  budgetForm.reset();
  editingBudgetId = null;
}

openBudgetModal.addEventListener("click", openModal);
emptyCreateBtn.addEventListener("click", openModal);
closeBudgetModal.addEventListener("click", closeModal);
cancelBudgetModal.addEventListener("click", closeModal);
budgetModal.addEventListener("click", function (event) {
  if (event.target === budgetModal) {
    closeModal();
  }
});

// Add or Update Budget

budgetForm.addEventListener("submit", function (event) {
  event.preventDefault();
  const name = document.getElementById("budgetName").value.trim();
  const category = document.getElementById("budgetCategory").value;
  const limit = Number(document.getElementById("budgetLimit").value);
  const period = document.getElementById("budgetPeriod").value;
  if (!name || !category || !Number.isFinite(limit) || limit <= 0) {
    alert("Please enter valid budget information.");
    return;
  }
  if (editingBudgetId !== null) {
    const budget = budgets.find(
      item => item.id === editingBudgetId
    );
    if (budget) {
      budget.name = name;
      budget.category = category;
      budget.limit = limit;
      budget.period = period;
    }
  } else {
    const newBudget = {
      id: Date.now(),
      name,
      category,
      limit,
      spent: 0,
      period
    };
    budgets.push(newBudget);
  }
  saveBudgets();
  updateSummary();
  filterBudgets();
  closeModal();
});

// Edit Budget

function editBudget(id) {
  const budget = budgets.find(item => item.id === id);
  if (!budget) return;
  editingBudgetId = id;
  document.getElementById("budgetName").value = budget.name;
  document.getElementById("budgetCategory").value = budget.category;
  document.getElementById("budgetLimit").value = budget.limit;
  document.getElementById("budgetPeriod").value = budget.period;
  document.getElementById("modalTitle").textContent =
    "Edit Budget";
  document.getElementById("saveBudgetBtn").textContent =
    "Update Budget";
  budgetModal.classList.add("show");
}

// Delete Budget

function deleteBudget(id) {
  const confirmed = confirm(
    "Are you sure you want to delete this budget?"
  );
  if (!confirmed) return;
  budgets = budgets.filter(item => item.id !== id);
  saveBudgets();
  updateSummary();
  filterBudgets();
}

// Escape HTML

function escapeHTML(value) {
  return String(value).replace(/[&<>"']/g, function (character) {
    const entities = {
      "&": "&amp;",
      "<": "&lt;",
      ">": "&gt;",
      '"': "&quot;",
      "'": "&#039;"
    };
    return entities[character];
  });
}

// Event Delegation

budgetGrid.addEventListener("click", function (event) {
  const editButton = event.target.closest(".edit-budget");
  const deleteButton = event.target.closest(".delete-budget");
  if (editButton) {
    editBudget(Number(editButton.dataset.id));
  }
  if (deleteButton) {
    deleteBudget(Number(deleteButton.dataset.id));
  }
});

// Sidebar Toggle

const sidebarToggle = document.getElementById("sidebarToggle");
const sidebar = document.getElementById("sidebar");
if (sidebarToggle && sidebar) {
  sidebarToggle.addEventListener("click", function () {
    sidebar.classList.toggle("open");
  });
}

// Initialize Page

saveBudgets();
updateSummary();
displayBudgets();
budgetFilter.addEventListener("change", filterBudgets);
lucide.createIcons();