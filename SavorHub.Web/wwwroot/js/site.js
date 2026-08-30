// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// ── Navbar offset ─────────────────────────────────────────────────────────────
// CSS cannot measure the real rendered navbar height, so JS does it.
// Runs on load and on every resize (navbar height changes when it collapses).
function syncNavbarOffset() {
    var header = document.querySelector('header.fixed-top');
    if (!header) return;
    var height = Math.ceil(header.getBoundingClientRect().height);
    var offset = height + 20;                                  // 20px visual gap
    document.body.style.paddingTop = offset + 'px';
    document.documentElement.style.scrollPaddingTop = offset + 'px';
}

document.addEventListener('DOMContentLoaded', syncNavbarOffset);
window.addEventListener('resize', syncNavbarOffset);

// ── Theme toggle ──────────────────────────────────────────────────────────────
// Applied early via inline script in _Layout.cshtml to prevent flash.
// This module wires up the button icon and click handler after DOM is ready.

function applyThemeIcon(theme) {
    var btn = document.getElementById('theme-toggle');
    if (!btn) return;
    btn.innerHTML = theme === 'dark'
        ? '<i class="bi bi-sun-fill"></i>'
        : '<i class="bi bi-moon-fill"></i>';
    btn.title = theme === 'dark' ? 'Switch to Light Mode' : 'Switch to Dark Mode';
}

document.addEventListener('DOMContentLoaded', function () {
    var current = document.documentElement.getAttribute('data-bs-theme') || 'light';
    applyThemeIcon(current);

    var btn = document.getElementById('theme-toggle');
    if (btn) {
        btn.addEventListener('click', function () {
            var html = document.documentElement;
            var next = html.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
            html.setAttribute('data-bs-theme', next);
            localStorage.setItem('theme', next);
            applyThemeIcon(next);
        });
    }
});
