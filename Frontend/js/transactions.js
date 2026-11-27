// Initial Data

const defaultTransactions = [
  {
    id: 1,
    description: "Monthly Salary",
    category: "Salary",
    type: "income",
    amount: 15000,
    date: "2026-10-28",
    note: "October salary"
  },
  {
    id: 2,
    description: "Grocery Shopping",
    category: "Food",
    type: "expense",
    amount: 1200,
    date: "2026-10-27",
    note: "Weekly groceries"
  },
  {
    id: 3,
    description: "Online Shopping",
    category: "Shopping",
    type: "expense",
    amount: 850,
    date: "2026-10-25",
    note: "Clothes and accessories"
  },
  {
    id: 4,
    description: "Freelance Project",
    category: "Freelance",
    type: "income",
    amount: 3500,
    date: "2026-10-23",
    note: "Website development"
  },
  {
    id: 5,
    description: "Transportation",
    category: "Transportation",
    type: "expense",
    amount: 300,
    date: "2026-10-20",
    note: "Transportation expenses"
  }
];

// Load Transactions

let transactions =
  JSON.parse(localStorage.getItem("pennywiseTransactions")) ||
  defaultTransactions;

// DOM Elements

const tableBody = document.getElementById("transactionsTableBody");
const emptyState = document.getElementById("emptyState");
const modal = document.getElementById("transactionModal");
const transactionForm = document.getElementById("transactionForm");
const modalTitle = document.getElementById("modalTitle");
const modalMessage = document.getElementById("modalMessage");
const openModalBtn = document.getElementById("openModalBtn");
const closeModalBtn = document.getElementById("closeModalBtn");
const cancelModalBtn = document.getElementById("cancelModalBtn");
const searchInput = document.getElementById("searchInput");
const typeFilter = document.getElementById("typeFilter");
const categoryFilter = document.getElementById("categoryFilter");
const sidebar = document.getElementById("sidebar");
const sidebarToggle = document.getElementById("sidebarToggle");
let editingId = null;

// Save to Local Storage

function saveTransactions() {
  localStorage.setItem(
    "pennywiseTransactions",
    JSON.stringify(transactions)
  );
}

// Format Currency

function formatCurrency(amount) {
  return new Intl.NumberFormat("en-EG", {
    style: "currency",
    currency: "EGP",
    minimumFractionDigits: 2
  }).format(amount);
}

// Format Date

function formatDate(date) {
  return new Date(date + "T00:00:00").toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric"
  });
}

// Update Summary

function updateSummary() {
  const totalIncome = transactions
    .filter(transaction => transaction.type === "income")
    .reduce((total, transaction) => total + transaction.amount, 0);
  const totalExpenses = transactions
    .filter(transaction => transaction.type === "expense")
    .reduce((total, transaction) => total + transaction.amount, 0);
  const netBalance = totalIncome - totalExpenses;
  document.getElementById("incomeTotal").textContent =
    formatCurrency(totalIncome);
  document.getElementById("expenseTotal").textContent =
    formatCurrency(totalExpenses);
  document.getElementById("netBalance").textContent =
    formatCurrency(netBalance);
  document.getElementById("transactionCount").textContent =
    transactions.length;
}

// Display Transactions

function displayTransactions(data) {
  tableBody.innerHTML = "";
  if (data.length === 0) {
    emptyState.hidden = false;
    lucide.createIcons();
    return;
  }
  emptyState.hidden = true;
  data.forEach(transaction => {
    const row = document.createElement("tr");
    const amountClass =
      transaction.type === "income"
      ? "income-amount"
      : "expense-amount";
    const amountSign =
      transaction.type === "income" ? "+" : "-";
    row.innerHTML = `
      <td>
        <span class="transaction-description">
          ${escapeHTML(transaction.description)}
        </span>
        <span class="transaction-note">
          ${escapeHTML(transaction.note || "No additional notes")}
        </span>
      </td>
      <td>
        <span class="category-badge">
          ${escapeHTML(transaction.category)}
        </span>
      </td>
      <td>
        <span class="type-badge type-${transaction.type}">
          ${transaction.type}
        </span>
      </td>
      <td>${formatDate(transaction.date)}</td>
      <td class="${amountClass}">
        ${amountSign} ${formatCurrency(transaction.amount)}
      </td>
      <td>
        <div class="action-buttons">
          <button
            class="action-btn edit-btn"
            data-action="edit"
            data-id="${transaction.id}"
            aria-label="Edit transaction"
          >
            <i data-lucide="pencil"></i>
          </button>
          <button
            class="action-btn delete-btn"
            data-action="delete"
            data-id="${transaction.id}"
            aria-label="Delete transaction"
          >
            <i data-lucide="trash-2"></i>
          </button>
        </div>
      </td>
      `;
    tableBody.appendChild(row);
  });
  lucide.createIcons();
}

// Prevent HTML Injection in Text

function escapeHTML(value) {
  return String(value).replace(/[&<>"']/g, function (character) {
    const characters = {
      "&": "&amp;",
      "<": "&lt;",
      ">": "&gt;",
      '"': "&quot;",
      "'": "&#039;"
    };
    return characters[character];
  });
}

// Search and Filter

function filterTransactions() {
  const searchValue = searchInput.value.toLowerCase().trim();
  const selectedType = typeFilter.value;
  const selectedCategory = categoryFilter.value;
  const filteredTransactions = transactions.filter(transaction => {
  const matchesSearch =
    transaction.description.toLowerCase().includes(searchValue) ||
    transaction.category.toLowerCase().includes(searchValue);
  const matchesType =
    selectedType === "all" ||
    transaction.type === selectedType;
  const matchesCategory =
    selectedCategory === "all" ||
    transaction.category === selectedCategory;
  return matchesSearch && matchesType && matchesCategory;
  });
  displayTransactions(filteredTransactions);
}

// Open Modal

function openModal() {
  modal.classList.add("active");
  modalMessage.textContent = "";
}

// Close Modal

function closeModal() {
  modal.classList.remove("active");
  transactionForm.reset();
  editingId = null;
  modalTitle.textContent = "Add Transaction";
  document.getElementById("saveTransactionBtn").textContent =
    "Save Transaction";
  modalMessage.textContent = "";
}

// Add Button

openModalBtn.addEventListener("click", function () {
  editingId = null;
  transactionForm.reset();
  modalTitle.textContent = "Add Transaction";
  document.getElementById("saveTransactionBtn").textContent =
    "Save Transaction";
  openModal();
});

// Close Buttons

closeModalBtn.addEventListener("click", closeModal);
cancelModalBtn.addEventListener("click", closeModal);
modal.addEventListener("click", function (event) {
  if (event.target === modal) {
    closeModal();
  }
});

// Open Add Transaction Modal from Dashboard
const urlParams = new URLSearchParams(window.location.search);

if (urlParams.get("add") === "true") {
  openModal();
}

// Add / Edit Transaction

transactionForm.addEventListener("submit", function (event) {
  event.preventDefault();
  const description =
    document.getElementById("description").value.trim();
  const amount =
    Number(document.getElementById("amount").value);
  const type =
    document.getElementById("transactionType").value;
  const category =
    document.getElementById("transactionCategory").value;
  const date =
    document.getElementById("transactionDate").value;
  const note =
    document.getElementById("transactionNote").value.trim();
  if (!description || !category || !date || !Number.isFinite(amount) || amount <= 0) {
    modalMessage.textContent = "Please enter valid transaction details.";
    return;
  }
  const transactionData = {
    description,
    amount,
    type,
    category,
    date,
    note
  };
  if (editingId !== null) {
    transactions = transactions.map(transaction => {
    if (transaction.id === editingId) {
      return {
        ...transaction,
        ...transactionData
      };
    }
    return transaction;
  });
  } else {
    const newTransaction = {
      id: Date.now(),
      ...transactionData
    };
    transactions.unshift(newTransaction);
  }
  saveTransactions();
  updateSummary();
  filterTransactions();
  closeModal();
});

// Edit / Delete

tableBody.addEventListener("click", function (event) {
  const button = event.target.closest("button[data-action]");
  if (!button) return;
  const id = Number(button.dataset.id);
  const action = button.dataset.action;
  if (action === "edit") {
    editTransaction(id);
  }
  if (action === "delete") {
    deleteTransaction(id);
  }
});

// Edit Transaction

function editTransaction(id) {
  const transaction = transactions.find(item => item.id === id);
  if (!transaction) return;
  editingId = id;
  document.getElementById("description").value =
    transaction.description;
  document.getElementById("amount").value =
    transaction.amount;
  document.getElementById("transactionType").value =
    transaction.type;
  document.getElementById("transactionCategory").value =
    transaction.category;
  document.getElementById("transactionDate").value =
    transaction.date;
  document.getElementById("transactionNote").value =
    transaction.note || "";
  modalTitle.textContent = "Edit Transaction";
  document.getElementById("saveTransactionBtn").textContent =
    "Update Transaction";
  openModal();
}

// Delete Transaction

function deleteTransaction(id) {
  const confirmed = confirm(
    "Are you sure you want to delete this transaction?"
  );
  if (!confirmed) return;
  transactions = transactions.filter(transaction => {
    return transaction.id !== id;
  });
  saveTransactions();
  updateSummary();
  filterTransactions();
}

// Search Events

searchInput.addEventListener("input", filterTransactions);
typeFilter.addEventListener("change", filterTransactions);
categoryFilter.addEventListener("change", filterTransactions);

// Sidebar

if (sidebar && sidebarToggle) {
  sidebarToggle.addEventListener("click", function () {
    sidebar.classList.toggle("open");
  });
}

// Initial Render

saveTransactions();
updateSummary();
filterTransactions();
lucide.createIcons();