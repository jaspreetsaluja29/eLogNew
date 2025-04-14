// Open the add new record window
function openAddWindow() {
    var basePath = document.querySelector('base')?.getAttribute('href') || '/';

    // Fetch user details from hidden fields
    var userId = document.getElementById('hiddenUserId')?.value || '';
    var userName = document.getElementById('hiddenUserName')?.value || '';
    var userRoleName = document.getElementById('hiddenUserRole')?.value || '';

    // Encode the values to handle spaces and special characters
    var url = `${basePath}/CodeA/DataEntry_CodeA?userId=${encodeURIComponent(userId)}&userName=${encodeURIComponent(userName)}&userRoleName=${encodeURIComponent(userRoleName)}`;

    window.open(url, 'AddRecord');
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
    var rows = document.querySelectorAll("#codeATable tbody tr");

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

    // Encode the values to handle spaces and special characters
    var url = `${basePath}/CodeA/Edit/${id}?userId=${encodeURIComponent(userId)}&userName=${encodeURIComponent(userName)}&userRoleName=${encodeURIComponent(userRoleName)}`;

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
        window.location.href = `${basePath}/ORB1/CodeA/GetCodeAData?pageNumber=${newPage}&pageSize=${pageSize}`;
    }
}

// Function to handle table downloads in different formats
function downloadTable(format) {
    // Show loading indicator
    showLoader();

    var basePath = document.querySelector('base')?.getAttribute('href') || '/';
    var searchTerm = encodeURIComponent($('#searchInput').val());
    var pageSize = $('#pageSize').val();
    var pageNumber = $('#currentPage').text();

    // Construct the download URL
    var downloadUrl = `${basePath}/ORB1/CodeA/Download?format=${format}&searchTerm=${searchTerm}&pageSize=${pageSize}&pageNumber=${pageNumber}`;

    // Create a temporary anchor element
    var a = document.createElement('a');
    a.style.display = 'none';
    a.href = downloadUrl;
    a.download = `CodeA_Records_${new Date().toISOString().slice(0, 10)}.${format}`;
    document.body.appendChild(a);

    // Trigger the download
    a.click();

    // Cleanup
    setTimeout(function () {
        document.body.removeChild(a);
        hideLoader();
    }, 3000);
}

// Helper function to show loading indicator
function showLoader() {
    if (!document.getElementById('downloadLoader')) {
        var loader = document.createElement('div');
        loader.id = 'downloadLoader';
        loader.className = 'download-loader';
        loader.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div><div class="mt-2">Preparing download...</div>';

        // Style the loader
        loader.style.position = 'fixed';
        loader.style.top = '50%';
        loader.style.left = '50%';
        loader.style.transform = 'translate(-50%, -50%)';
        loader.style.backgroundColor = 'rgba(255, 255, 255, 0.9)';
        loader.style.padding = '20px';
        loader.style.borderRadius = '5px';
        loader.style.boxShadow = '0 0 10px rgba(0, 0, 0, 0.1)';
        loader.style.zIndex = '9999';
        loader.style.textAlign = 'center';

        document.body.appendChild(loader);
    } else {
        document.getElementById('downloadLoader').style.display = 'block';
    }
}

// Helper function to hide loading indicator
function hideLoader() {
    var loader = document.getElementById('downloadLoader');
    if (loader) {
        loader.style.display = 'none';
    }
}