// Open the add new record window
function openAddWindow() {
    window.open('/CodeA/DataEntry_CodeA', 'AddRecord');
}

// Track sorting order
let sortOrder = {};

function sortTable(columnIndex) {
    var table = document.getElementById("codeATable");
    var rows = Array.from(table.rows).slice(1); // Skip header row
    var sortedRows;

    // Toggle sorting order
    sortOrder[columnIndex] = !sortOrder[columnIndex];

    sortedRows = rows.sort((rowA, rowB) => {
        let cellA = rowA.cells[columnIndex].textContent.trim(); // Use textContent instead of innerText
        let cellB = rowB.cells[columnIndex].textContent.trim();

        // **Fix "Entered By" Sorting**
        if (columnIndex === 1) {
            return sortOrder[columnIndex]
                ? cellA.localeCompare(cellB, undefined, { sensitivity: 'base' })
                : cellB.localeCompare(cellA, undefined, { sensitivity: 'base' });
        }

        // Handle Date Sorting
        if ([2, 4, 10, 12, 18, 20].includes(columnIndex)) {
            let dateA = new Date(cellA);
            let dateB = new Date(cellB);
            if (isNaN(dateA) || isNaN(dateB)) return 0;
            return sortOrder[columnIndex] ? dateA - dateB : dateB - dateA;
        }

        // Handle Number Sorting
        if ([0, 9, 17].includes(columnIndex)) {
            return sortOrder[columnIndex] ? parseFloat(cellA) - parseFloat(cellB) : parseFloat(cellB) - parseFloat(cellA);
        }

        // Handle Boolean Sorting (Convert "true"/"false" to 1/0)
        if ([7].includes(columnIndex)) {
            return sortOrder[columnIndex] ? (cellA === "true") - (cellB === "true") : (cellB === "true") - (cellA === "true");
        }

        // Default: String Sorting
        return sortOrder[columnIndex] ? cellA.localeCompare(cellB) : cellB.localeCompare(cellA);
    });

    // Reorder the table with the sorted rows
    table.tBodies[0].append(...sortedRows);

    // Update row count
    updateRowCount();
}

// Filter the table based on the search input
function filterTable() {
    var input = document.getElementById("searchInput").value.toLowerCase();
    var rows = document.querySelectorAll("#codeATable tbody tr");

    rows.forEach(row => {
        row.style.display = row.innerText.toLowerCase().includes(input) ? "" : "none";
    });

    // Update row count after filtering
    updateRowCount();
}

// Update the row count dynamically based on visible rows
function updateRowCount() {
    var rowCountElement = document.getElementById("rowCount");

    if (!rowCountElement) {
        console.error("Element with ID 'rowCount' not found!");
        return;
    }

    var rows = document.querySelectorAll("#codeATable tbody tr");
    var visibleRows = Array.from(rows).filter(row => row.style.display !== "none").length;
    rowCountElement.textContent = `Total Records: ${visibleRows}`;
}

// Open the edit window for a record
function editRecord(id) {
    // Open the edit page for the selected record
    window.open(`/CodeA/Edit/${id}`, 'EditRecord');
}


