// Add to your site.js
$(document).ready(function () {
    // Check if we're on the home page
    if (window.location.pathname.endsWith("/Home/Index") ||
        window.location.pathname.endsWith("/") ||
        window.location.pathname.includes("DigitalLog")) {
        $("#navbarLogo").addClass("logo-large");
    }

    // Add click event to resize logo when clicking on home links
    $("a[href='/Home/Index'], a:contains('DigitalLog')").click(function () {
        $("#navbarLogo").addClass("logo-large");
    });
});