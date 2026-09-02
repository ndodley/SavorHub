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

// ── In-place form actions (cart buttons, reviews, etc.) ─────────────────────────
// Any <form data-ajax-form> submits into the page as normal so it still works with
// JS disabled. When JS is available we intercept the submit, replay it as a fetch
// so the resulting redirect never navigates the browser, then swap only the markup
// inside the closest [data-ajax-region] ancestor - this is what keeps actions like
// Add to Cart, the qty +/- buttons, and posting a review from feeling like a full
// page reload, and keeps you on the page you were already on. Falls back to a real
// form submission if anything about the fetch/parse fails.
document.addEventListener('submit', function (e) {
    // Respect an inline onsubmit="return confirm(...)" on the form (used by the
    // Delete Review button) - if the user cancelled it, the submit is already
    // prevented and we must not fire the request anyway.
    if (e.defaultPrevented) return;

    var form = e.target.closest('[data-ajax-form]');
    if (!form) return;

    var region = form.closest('[data-ajax-region]');
    if (!region) return; // no region to swap into - fall back to a normal submit

    e.preventDefault();

    fetch(form.action, {
        method: 'POST',
        credentials: 'same-origin',
        body: new FormData(form)
    })
        .then(function (response) {
            if (!response.ok) {
                throw new Error('Request failed: ' + response.status);
            }
            return response.text();
        })
        .then(function (html) {
            var key = region.getAttribute('data-ajax-region');
            var doc = new DOMParser().parseFromString(html, 'text/html');
            var newRegion = doc.querySelector('[data-ajax-region="' + key + '"]');
            if (!newRegion) {
                if (region.hasAttribute('data-ajax-remove-if-missing')) {
                    // The region no longer exists on the server's re-render at
                    // all - e.g. an item was deleted from a list. Treat that as
                    // success and fade the whole region out instead of falling
                    // back to a real (and pointless) page reload.
                    region.style.transition = 'opacity 0.2s ease';
                    region.style.opacity = '0';
                    setTimeout(function () { region.remove(); }, 200);
                } else {
                    // Most likely an anonymous visitor got redirected to the login
                    // page instead of the action going through - let a real submit
                    // follow that redirect so they see the login form.
                    form.submit();
                }
                return;
            }
            region.innerHTML = newRegion.innerHTML;

            // Carry over any success/error toast the server queued (TempData),
            // since it lives outside the swapped region and would otherwise be lost.
            if (window.toastr) {
                var successEl = doc.getElementById('ajax-toast-success');
                if (successEl) {
                    toastr.success(successEl.textContent);
                }
                var errorEl = doc.getElementById('ajax-toast-error');
                if (errorEl) {
                    toastr.error(errorEl.textContent);
                }
            }
        })
        .catch(function () {
            // Network hiccup or unexpected response - fall back to a real submit
            // so the action still goes through.
            form.submit();
        });
});
