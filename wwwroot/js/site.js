/* ============================================================
   Bayan — interface behaviour
   Theme, navigation, dialogs, likes, uploads and small helpers.
   ============================================================ */
(function () {
    "use strict";

    var BAYAN = window.BAYAN || {};
    var strings = BAYAN.strings || {};
    var csrfMeta = document.querySelector('meta[name="csrf-token"]');
    var csrfToken = csrfMeta ? csrfMeta.getAttribute("content") : "";

    function $(selector, root) {
        return (root || document).querySelector(selector);
    }

    function $all(selector, root) {
        return Array.prototype.slice.call((root || document).querySelectorAll(selector));
    }

    function on(target, event, handler) {
        if (target) target.addEventListener(event, handler);
    }

    /* ---------- Theme ---------- */

    function initTheme() {
        var toggle = document.querySelector("[data-theme-toggle]");
        if (!toggle) return;

        function sync() {
            var isDark = document.documentElement.getAttribute("data-theme") === "dark";
            toggle.setAttribute("aria-pressed", isDark ? "true" : "false");
            var label = isDark
                ? (BAYAN.culture === "ar" ? "التبديل إلى المظهر الفاتح" : "Switch to light theme")
                : (BAYAN.culture === "ar" ? "التبديل إلى المظهر الداكن" : "Switch to dark theme");
            toggle.setAttribute("aria-label", label);
            toggle.setAttribute("title", label);
        }

        sync();

        toggle.addEventListener("click", function () {
            var isDark = document.documentElement.getAttribute("data-theme") === "dark";
            if (isDark) {
                document.documentElement.removeAttribute("data-theme");
            } else {
                document.documentElement.setAttribute("data-theme", "dark");
            }

            try {
                localStorage.setItem("bayan-theme", isDark ? "light" : "dark");
            } catch (e) { /* storage may be blocked */ }

            sync();
        });
    }

    /* ---------- Header + mobile navigation ---------- */

    function initHeader() {
        var header = document.getElementById("siteHeader");
        if (header) {
            var update = function () {
                header.classList.toggle("is-scrolled", window.scrollY > 8);
            };
            update();
            window.addEventListener("scroll", update, { passive: true });
        }

        var nav = document.getElementById("siteNav");
        var toggler = document.querySelector(".nav-toggle");
        if (nav && toggler) {
            toggler.addEventListener("click", function () {
                var expanded = toggler.getAttribute("aria-expanded") === "true";
                toggler.setAttribute("aria-expanded", expanded ? "false" : "true");
            });

            $all("a", nav).forEach(function (link) {
                link.addEventListener("click", function () {
                    if (window.innerWidth >= 1200) return;
                    if (window.bootstrap && window.bootstrap.Collapse) {
                        var instance = window.bootstrap.Collapse.getInstance(nav);
                        if (instance) instance.hide();
                    } else {
                        nav.classList.remove("show");
                    }
                    toggler.setAttribute("aria-expanded", "false");
                });
            });
        }
    }

    /* ---------- Toasts ---------- */

    function toast(message, type) {
        if (!message) return;
        var region = document.getElementById("toastRegion");
        if (!region) return;

        var el = document.createElement("div");
        el.className = "app-toast" + (type === "error" ? " toast-error" : "");
        el.setAttribute("role", type === "error" ? "alert" : "status");

        var icon = document.createElement("span");
        icon.className = "app-toast-icon";
        icon.innerHTML = type === "error"
            ? '<svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M12 4 2.8 20h18.4L12 4Z"/><path d="M12 9v5"/><circle cx="12" cy="17" r=".9" fill="currentColor" stroke="none"/></svg>'
            : '<svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m5 12 4.5 4.5L19 7"/></svg>';

        var text = document.createElement("p");
        text.textContent = message;

        el.appendChild(icon);
        el.appendChild(text);
        region.appendChild(el);

        var remove = function () {
            el.classList.add("is-leaving");
            window.setTimeout(function () {
                if (el.parentNode) el.parentNode.removeChild(el);
            }, 190);
        };

        window.setTimeout(remove, 4800);
        el.addEventListener("click", remove);
    }

    /* ---------- Dialogs ---------- */

    function confirmDialog(options) {
        options = options || {};
        var dialog = document.getElementById("confirmDialog");
        if (!dialog || typeof dialog.showModal !== "function") {
            return Promise.resolve(window.confirm(options.text || strings.confirm || "Are you sure?"));
        }

        var title = document.getElementById("confirmTitle");
        var text = document.getElementById("confirmText");
        var accept = document.getElementById("confirmAccept");
        var cancel = document.getElementById("confirmCancel");

        title.textContent = options.title || strings.confirm || "Confirm";
        text.textContent = options.text || "";
        accept.textContent = options.accept || strings.confirm || "Confirm";
        cancel.textContent = options.cancel || strings.cancel || "Cancel";

        return new Promise(function (resolve) {
            var settled = false;

            function finish(value) {
                if (settled) return;
                settled = true;
                dialog.removeEventListener("close", onClose);
                resolve(value);
            }

            function onClose() {
                finish(dialog.returnValue === "confirm");
            }

            dialog.addEventListener("close", onClose);
            if (dialog.open) dialog.close("cancel");
            dialog.showModal();
            accept.focus();

            cancel.onclick = function () { dialog.close("cancel"); };
            accept.onclick = function () { dialog.close("confirm"); };
        });
    }

    function promptDialog(options) {
        options = options || {};
        var dialog = document.getElementById("promptDialog");
        if (!dialog || typeof dialog.showModal !== "function") {
            return Promise.resolve(window.prompt(options.label || "", options.value || ""));
        }

        var title = document.getElementById("promptTitle");
        var label = document.getElementById("promptLabel");
        var input = document.getElementById("promptInput");
        var secondaryField = document.getElementById("promptSecondaryField");
        var secondaryLabel = document.getElementById("promptSecondaryLabel");
        var secondary = document.getElementById("promptSecondary");

        title.textContent = options.title || strings.insert || "Insert";
        label.textContent = options.label || "";
        input.value = options.value || "";
        secondaryField.hidden = !options.secondaryLabel;
        secondaryLabel.textContent = options.secondaryLabel || "";
        secondary.value = options.secondaryValue || "";

        return new Promise(function (resolve) {
            var settled = false;

            function finish(value) {
                if (settled) return;
                settled = true;
                dialog.removeEventListener("close", onClose);
                resolve(value);
            }

            function onClose() {
                finish(dialog.returnValue === "confirm" ? input.value.trim() : null);
            }

            function onKeyDown(event) {
                if (event.key === "Enter") {
                    event.preventDefault();
                    dialog.close("confirm");
                }
            }

            dialog.addEventListener("close", onClose);
            input.addEventListener("keydown", onKeyDown);
            if (dialog.open) dialog.close("cancel");
            dialog.showModal();
            window.setTimeout(function () { input.focus(); input.select(); }, 20);
        });
    }

    function initConfirmForms() {
        document.addEventListener("submit", function (event) {
            var form = event.target;
            if (!form || form.nodeName !== "FORM") return;
            if (!form.hasAttribute("data-confirm")) return;
            if (form.dataset.confirmed === "true") return;

            event.preventDefault();

            confirmDialog({
                title: form.getAttribute("data-confirm-title") || strings.deleteTitle || strings.confirm,
                text: form.getAttribute("data-confirm-text") || strings.deleteText || "",
                accept: form.getAttribute("data-confirm-accept") || strings.deleteConfirm || strings.confirm,
                cancel: form.getAttribute("data-confirm-cancel") || strings.deleteCancel || strings.cancel
            }).then(function (ok) {
                if (!ok) return;
                form.dataset.confirmed = "true";
                if (typeof form.requestSubmit === "function") {
                    form.requestSubmit();
                } else {
                    form.submit();
                }
            });
        });
    }

    /* ---------- Flash messages ---------- */

    function initFlash() {
        $all("[data-dismiss-flash]").forEach(function (button) {
            button.addEventListener("click", function () {
                var flash = button.closest(".flash");
                if (flash) flash.remove();
            });
        });

        $all(".flash").forEach(function (flash) {
            window.setTimeout(function () {
                flash.style.transition = "opacity 220ms ease, transform 220ms ease";
                flash.style.opacity = "0";
                flash.style.transform = "translateY(-4px)";
                window.setTimeout(function () { flash.remove(); }, 240);
            }, 6500);
        });
    }

    /* ---------- Back to top ---------- */

    function initBackToTop() {
        var button = document.getElementById("backToTop");
        if (!button) return;

        var update = function () {
            button.classList.toggle("is-visible", window.scrollY > 420);
        };

        update();
        window.addEventListener("scroll", update, { passive: true });
        button.addEventListener("click", function () {
            window.scrollTo({ top: 0, behavior: "smooth" });
        });
    }

    /* ---------- Copy link ---------- */

    function copyText(value) {
        if (navigator.clipboard && window.isSecureContext) {
            return navigator.clipboard.writeText(value);
        }

        return new Promise(function (resolve, reject) {
            var area = document.createElement("textarea");
            area.value = value;
            area.setAttribute("readonly", "");
            area.style.position = "fixed";
            area.style.opacity = "0";
            document.body.appendChild(area);
            area.select();
            try {
                document.execCommand("copy");
                resolve();
            } catch (error) {
                reject(error);
            } finally {
                document.body.removeChild(area);
            }
        });
    }

    function initCopyLinks() {
        $all("[data-copy-link]").forEach(function (button) {
            button.addEventListener("click", function () {
                var value = button.getAttribute("data-copy-value") || window.location.href;
                copyText(value).then(function () {
                    toast(strings.copied || "Link copied");
                }).catch(function () {
                    toast(strings.error || "Something went wrong", "error");
                });
            });
        });
    }

    /* ---------- Likes ---------- */

    function initLikes() {
        $all("[data-like-button]").forEach(function (button) {
            button.addEventListener("click", function () {
                if (button.dataset.busy === "true") return;

                var postId = button.getAttribute("data-post-id");
                if (!postId) return;

                button.dataset.busy = "true";
                button.classList.add("is-busy");

                fetch("/api/posts/" + encodeURIComponent(postId) + "/like", {
                    method: "POST",
                    headers: {
                        "RequestVerificationToken": csrfToken,
                        "X-Requested-With": "XMLHttpRequest"
                    },
                    credentials: "same-origin"
                }).then(function (response) {
                    if (response.status === 401) {
                        toast((strings.loginRequired || "Sign in required") + ". " + (strings.loginRequiredText || ""), "error");
                        throw new Error("unauthorized");
                    }
                    if (!response.ok) throw new Error("request failed");
                    return response.json();
                }).then(function (data) {
                    var liked = !!data.liked;
                    var count = typeof data.count === "number" ? data.count : 0;

                    button.classList.toggle("is-liked", liked);
                    button.setAttribute("aria-pressed", liked ? "true" : "false");
                    button.setAttribute("data-liked", liked ? "true" : "false");

                    var label = $(".like-label", button);
                    if (label) label.textContent = data.label || (liked ? "Liked" : "Like");

                    var countEl = $(".like-count", button);
                    if (countEl) countEl.textContent = String(count);

                    var icon = $(".like-icon", button);
                    if (icon) {
                        // Re-render the outline/filled heart without relying on icon fonts.
                        icon.innerHTML = liked
                            ? '<svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="19" height="19" fill="currentColor" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M12 20.5S4 15.5 4 9.8A4.3 4.3 0 0 1 12 7a4.3 4.3 0 0 1 8 2.8c0 5.7-8 10.7-8 10.7Z"/></svg>'
                            : '<svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="19" height="19" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M12 20.5S4 15.5 4 9.8A4.3 4.3 0 0 1 12 7a4.3 4.3 0 0 1 8 2.8c0 5.7-8 10.7-8 10.7Z"/></svg>';
                    }
                }).catch(function (error) {
                    if (error && error.message === "unauthorized") return;
                    toast(strings.likeError || strings.error || "Could not update your like.", "error");
                }).finally(function () {
                    button.dataset.busy = "false";
                    button.classList.remove("is-busy");
                });
            });
        });
    }

    /* ---------- Comments ---------- */

    function initComments() {
        $all("[data-comment-edit]").forEach(function (button) {
            button.addEventListener("click", function () {
                var id = button.getAttribute("data-comment-edit");
                var content = document.getElementById("comment-content-" + id);
                var form = document.getElementById("comment-edit-" + id);
                if (!form) return;

                if (content) content.hidden = true;
                form.hidden = false;

                var textarea = form.querySelector("textarea");
                if (textarea) {
                    textarea.focus();
                    textarea.setSelectionRange(textarea.value.length, textarea.value.length);
                }
            });
        });

        $all("[data-comment-cancel]").forEach(function (button) {
            button.addEventListener("click", function () {
                var id = button.getAttribute("data-comment-cancel");
                var content = document.getElementById("comment-content-" + id);
                var form = document.getElementById("comment-edit-" + id);
                if (form) form.hidden = true;
                if (content) content.hidden = false;
            });
        });
    }

    /* ---------- Character counters ---------- */

    function initCounters() {
        $all("[data-char-counter-for]").forEach(function (counter) {
            var id = counter.getAttribute("data-char-counter-for");
            var input = document.getElementById(id);
            if (!input) return;

            var max = input.getAttribute("maxlength");
            var render = function () {
                var length = input.value.length;
                counter.textContent = max ? length + " / " + max : String(length);
            };

            render();
            input.addEventListener("input", render);
        });
    }

    /* ---------- Password visibility ---------- */

    function initPasswordToggles() {
        $all("[data-password-toggle]").forEach(function (button) {
            button.addEventListener("click", function () {
                var field = button.closest(".password-field");
                var input = field ? field.querySelector("input") : null;
                if (!input) return;

                var show = input.type === "password";
                input.type = show ? "text" : "password";
                button.classList.toggle("is-visible", show);

                var label = show
                    ? (strings.hidePassword || "Hide password")
                    : (strings.showPassword || "Show password");
                button.setAttribute("aria-label", label);
                button.setAttribute("title", label);
            });
        });
    }

    /* ---------- Cover uploader ---------- */

    function initCoverUploader() {
        $all("[data-cover-uploader]").forEach(function (wrap) {
            var input = wrap.querySelector("[data-cover-input]");
            var trigger = wrap.querySelector("[data-cover-trigger]");
            var remove = wrap.querySelector("[data-cover-remove]");
            var preview = wrap.querySelector("[data-cover-preview]");
            var image = wrap.querySelector("[data-cover-image]");
            var removeFlag = wrap.querySelector("[data-cover-remove-input]");
            var hasExisting = wrap.getAttribute("data-has-existing") === "true";

            if (!input) return;

            function showPreview(file) {
                if (!file) return;
                if (file.size > 5 * 1024 * 1024) {
                    toast(BAYAN.culture === "ar" ? "حجم الصورة يجب ألا يزيد عن 5 ميجابايت." : "Images must be 5 MB or smaller.", "error");
                    input.value = "";
                    return;
                }

                if (image) {
                    image.src = URL.createObjectURL(file);
                    image.hidden = false;
                } else if (preview) {
                    var img = document.createElement("img");
                    img.setAttribute("data-cover-image", "");
                    img.alt = "";
                    img.src = URL.createObjectURL(file);
                    preview.insertBefore(img, preview.firstChild);
                }

                wrap.classList.add("has-image");
                if (remove) remove.hidden = false;
                if (removeFlag) removeFlag.value = "false";
            }

            if (trigger) {
                trigger.addEventListener("click", function () { input.click(); });
            }

            input.addEventListener("change", function () {
                if (input.files && input.files[0]) showPreview(input.files[0]);
            });

            if (remove) {
                remove.hidden = !wrap.classList.contains("has-image");
                remove.addEventListener("click", function () {
                    input.value = "";
                    wrap.classList.remove("has-image");
                    remove.hidden = true;
                    if (removeFlag && hasExisting) removeFlag.value = "true";

                    var img = wrap.querySelector("[data-cover-image]");
                    if (img) {
                        img.removeAttribute("src");
                        img.hidden = true;
                    }
                });
            }
        });
    }

    /* ---------- Generic image preview ---------- */

    function initImagePreviews() {
        $all("[data-image-input]").forEach(function (input) {
            input.addEventListener("change", function () {
                var file = input.files && input.files[0];
                if (!file) return;

                if (file.size > 5 * 1024 * 1024) {
                    toast(BAYAN.culture === "ar" ? "حجم الصورة يجب ألا يزيد عن 5 ميجابايت." : "Images must be 5 MB or smaller.", "error");
                    input.value = "";
                    return;
                }

                var targets = [];
                var primary = input.getAttribute("data-preview");
                var secondary = input.getAttribute("data-preview-second");
                if (primary) targets.push(primary);
                if (secondary) targets.push(secondary);

                targets.forEach(function (selector) {
                    $all(selector).forEach(function (img) {
                        img.src = URL.createObjectURL(file);
                        img.hidden = false;
                        var wrap = img.closest("[data-preview-wrap]");
                        if (wrap) wrap.classList.add("has-image");
                    });
                });
            });
        });
    }

    /* ---------- Public helper ---------- */

    window.BayanUI = {
        toast: toast,
        confirm: confirmDialog,
        prompt: promptDialog,
        copy: copyText
    };

    /* ---------- Boot ---------- */

    function boot() {
        initTheme();
        initHeader();
        initConfirmForms();
        initFlash();
        initBackToTop();
        initCopyLinks();
        initLikes();
        initComments();
        initCounters();
        initPasswordToggles();
        initCoverUploader();
        initImagePreviews();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", boot);
    } else {
        boot();
    }
})();
