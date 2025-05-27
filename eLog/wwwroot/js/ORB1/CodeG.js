// Open the add new record window
function openAddWindow() {
    var basePath = document.querySelector('base')?.getAttribute('href') || '/';

    // Fetch user details from hidden fields
    var userId = document.getElementById('hiddenUserId')?.value || '';
    var userName = document.getElementById('hiddenUserName')?.value || '';
    var userRoleName = document.getElementById('hiddenUserRole')?.value || '';
    var jobRank = document.getElementById('hiddenJobRank')?.value || '';

    // Encode the values to handle spaces and special characters
    var url = `${basePath}/CodeG/DataEntry_CodeG?userId=${encodeURIComponent(userId)}&userName=${encodeURIComponent(userName)}&userRoleName=${encodeURIComponent(userRoleName)}&jobRank=${encodeURIComponent(jobRank)}`;

    window.open(url, 'AddRecord');
}

// Track sorting order
let sortOrder = {};
function sortTable(columnIndex) {
    var table = document.getElementById("codeGTable");
    var rows = Array.from(table.rows).slice(1); // Skip header row
    var sortedRows;

    // Toggle sorting order
    sortOrder[columnIndex] = !sortOrder[columnIndex];

    sortedRows = rows.sort((rowA, rowB) => {
        let cellA = rowA.cells[columnIndex].textContent.trim(); // Use textContent instead of innerText
        let cellB = rowB.cells[columnIndex].textContent.trim();

        // **Fix "Entered By" Sorting**
        if (columnIndex === 0) { // Changed from 1 to 0 to match the actual column index
            return sortOrder[columnIndex]
                ? cellA.localeCompare(cellB, undefined, { sensitivity: 'base' })
                : cellB.localeCompare(cellA, undefined, { sensitivity: 'base' });
        }

        // Handle Date Sorting
        if ([1, 3, 10, 12, 18, 20].includes(columnIndex)) { // Updated column indices to match table
            let dateA = new Date(cellA);
            let dateB = new Date(cellB);
            if (isNaN(dateA) || isNaN(dateB)) return 0;
            return sortOrder[columnIndex] ? dateA - dateB : dateB - dateA;
        }

        // Handle Number Sorting
        if ([9, 17].includes(columnIndex)) { // Updated column indices
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
}

// Filter the table based on the search input
function filterTable() {
    var input = document.getElementById("searchInput").value.toLowerCase();
    var rows = document.querySelectorAll("#codeGTable tbody tr");

    rows.forEach(row => {
        row.style.display = row.innerText.toLowerCase().includes(input) ? "" : "none";
    });
}

function editRecord(id) {
    var basePath = document.querySelector('base')?.getAttribute('href') || '/';

    // Fetch user details from hidden fields
    var userId = document.getElementById('hiddenUserId')?.value || '';
    var userName = document.getElementById('hiddenUserName')?.value || '';
    var userRoleName = document.getElementById('hiddenUserRole')?.value || '';
    var jobRank = document.getElementById('hiddenJobRank')?.value || '';

    // Encode the values to handle spaces and special characters
    var url = `${basePath}/CodeG/Edit/${id}?userId=${encodeURIComponent(userId)}&userName=${encodeURIComponent(userName)}&userRoleName=${encodeURIComponent(userRoleName)}&jobRank=${encodeURIComponent(jobRank)}`;

    window.open(url, 'EditRecord'); // Open in new window/tab
}



// Pagination Logic
function changePage(offset) {
    let currentPage = parseInt(document.getElementById("currentPage").textContent);
    let totalPages = parseInt(document.getElementById("totalPages").textContent);
    let pageSize = document.getElementById("pageSize").value;

    let newPage = currentPage + offset;

    if (newPage >= 1 && newPage <= totalPages) {
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeG/GetCodeGData?pageNumber=${newPage}&pageSize=${pageSize}`;
    }
}