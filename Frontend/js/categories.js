// Default Categories

const defaultCategories = [
  { id: 1, name: "Food", type: "Expense", icon: "utensils" },
  { id: 2, name: "Shopping", type: "Expense", icon: "shopping-bag" },
  { id: 3, name: "Transportation", type: "Expense", icon: "car" },
  { id: 4, name: "Health", type: "Expense", icon: "heart-pulse" },
  { id: 5, name: "Education", type: "Expense", icon: "graduation-cap" },
  { id: 6, name: "Entertainment", type: "Expense", icon: "gamepad-2" },
  { id: 7, name: "Salary", type: "Income", icon: "briefcase" },
  { id: 8, name: "Freelance", type: "Income", icon: "wallet" }
];

let categories = JSON.parse(
  localStorage.getItem("pennywiseCategories")
) || defaultCategories;

let editingCategoryId = null;

// DOM Elements

const categoriesGrid = document.getElementById("categoriesGrid");
const categoryModal = document.getElementById("categoryModal");
const categoryForm = document.getElementById("categoryForm");
const categoryFilter = document.getElementById("categoryFilter");

// Save Categories

function saveCategories() {
  localStorage.setItem(
    "pennywiseCategories",
    JSON.stringify(categories)
  );
}

// Update Summary

function updateSummary() {
  document.getElementById("totalCategories").textContent =
    categories.length;
  document.getElementById("incomeCategories").textContent =
    categories.filter(category => category.type === "Income").length;
  document.getElementById("expenseCategories").textContent =
    categories.filter(category => category.type === "Expense").length;
}

// Display Categories

function displayCategories(data = categories) {
  categoriesGrid.innerHTML = "";
  const emptyState = document.getElementById("categoryEmpty");
  if (data.length === 0) {
    emptyState.hidden = false;
    lucide.createIcons();
    return;
  }
  emptyState.hidden = true;
  data.forEach(category => {
    const card = document.createElement("article");
    card.className = "category-card";
    card.innerHTML = `
      <div class="category-card-top">
        <div class="category-card-icon">
          <i data-lucide="${escapeHTML(category.icon)}"></i>
        </div>
        <div class="category-card-actions">
          <button class="edit-category" data-id="${category.id}" aria-label="Edit category">
            <i data-lucide="pencil"></i>
          </button>
          <button class="delete-category" data-id="${category.id}" aria-label="Delete category">
            <i data-lucide="trash-2"></i>
          </button>
        </div>
      </div>
      <h3>${escapeHTML(category.name)}</h3>
      <span class="category-type ${category.type.toLowerCase()}">
        ${category.type}
      </span>
    `;
    categoriesGrid.appendChild(card);
  });
  lucide.createIcons();
}

// Filter Categories

function filterCategories() {
  const selectedType = categoryFilter.value;
  const filtered = selectedType === "all"
    ? categories
    : categories.filter(category => category.type === selectedType);
  displayCategories(filtered);
}

// Modal Functions

function openCategoryModal() {
  categoryForm.reset();
  editingCategoryId = null;
  document.getElementById("categoryModalTitle").textContent =
    "Add New Category";
  document.getElementById("saveCategoryBtn").textContent =
    "Save Category";
  categoryModal.classList.add("show");
}

function closeCategoryModal() {
  categoryModal.classList.remove("show");
  categoryForm.reset();
  editingCategoryId = null;
}

document.getElementById("openCategoryModal")
  .addEventListener("click", openCategoryModal);
document.getElementById("closeCategoryModal")
  .addEventListener("click", closeCategoryModal);
document.getElementById("cancelCategoryModal")
  .addEventListener("click", closeCategoryModal);

categoryModal.addEventListener("click", event => {
  if (event.target === categoryModal) {
    closeCategoryModal();
  }
});

// Add / Update Category

categoryForm.addEventListener("submit", event => {
  event.preventDefault();
  const name = document.getElementById("categoryName").value.trim();
  const type = document.getElementById("categoryType").value;
  const icon = document.getElementById("categoryIcon").value;
  if (!name) return;
  const duplicate = categories.some(category =>
    category.name.toLowerCase() === name.toLowerCase() &&
    category.id !== editingCategoryId
  );
  if (duplicate) {
    alert("This category already exists.");
    return;
  }
  if (editingCategoryId !== null) {
    const category = categories.find(
      item => item.id === editingCategoryId
    );
  if (category) {
    category.name = name;
    category.type = type;
    category.icon = icon;
  }
  } else {
    categories.push({
      id: Date.now(),
      name,
      type,
      icon
    });
  }
  saveCategories();
  updateSummary();
  filterCategories();
  closeCategoryModal();
});

// Edit Category

function editCategory(id) {
  const category = categories.find(item => item.id === id);
  if (!category) return;
  editingCategoryId = id;
  document.getElementById("categoryName").value = category.name;
  document.getElementById("categoryType").value = category.type;
  document.getElementById("categoryIcon").value = category.icon;
  document.getElementById("categoryModalTitle").textContent =
    "Edit Category";
  document.getElementById("saveCategoryBtn").textContent =
    "Update Category";
  categoryModal.classList.add("show");
}

// Delete Category

function deleteCategory(id) {
  const confirmed = confirm(
    "Are you sure you want to delete this category?"
  );
  if (!confirmed) return;
  categories = categories.filter(category => category.id !== id);
  saveCategories();
  updateSummary();
  filterCategories();
}

// Escape HTML

function escapeHTML(value) {
  return String(value).replace(/[&<>"']/g, character => ({
    "&": "&amp;",
    "<": "&lt;",
    ">": "&gt;",
    '"': "&quot;",
    "'": "&#039;"
  })[character]);
}

// Event Delegation

categoriesGrid.addEventListener("click", event => {
  const editButton = event.target.closest(".edit-category");
  const deleteButton = event.target.closest(".delete-category");
  if (editButton) {
    editCategory(Number(editButton.dataset.id));
  }
  if (deleteButton) {
    deleteCategory(Number(deleteButton.dataset.id));
  }
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

saveCategories();
updateSummary();
displayCategories();
categoryFilter.addEventListener("change", filterCategories);
lucide.createIcons();