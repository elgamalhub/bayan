/* ============================================================
   Bayan — built-in rich text editor
   No external editor SDK, no API keys. Uses contenteditable with
   a restrained command set and a local image-upload endpoint.
   ============================================================ */
(function () {
    "use strict";

    var BAYAN = window.BAYAN || {};
    var strings = BAYAN.strings || {};
    var isRtl = !!BAYAN.isRtl;
    var csrfMeta = document.querySelector('meta[name="csrf-token"]');
    var csrfToken = csrfMeta ? csrfMeta.getAttribute("content") : "";
    var editors = [];

    function text(key, fallback) {
        return strings[key] || fallback;
    }

    function escapeHtml(value) {
        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function isSafeLink(value) {
        if (!value) return false;
        var trimmed = value.trim();
        if (trimmed.charAt(0) === "/" || trimmed.charAt(0) === "#") {
            return trimmed.indexOf("//") !== 0;
        }
        return /^(https?:\/\/|mailto:)/i.test(trimmed);
    }

    function normalizeUrl(value) {
        var trimmed = String(value || "").trim();
        if (!trimmed) return "";
        if (/^[a-z][a-z0-9+.-]*:/i.test(trimmed) || trimmed.charAt(0) === "/" || trimmed.charAt(0) === "#") {
            return trimmed;
        }
        return "https://" + trimmed;
    }

    function formatCount(words, characters) {
        if (isRtl) {
            return words + " كلمة · " + characters + " حرفًا";
        }
        return words + " words · " + characters + " characters";
    }

    function setup(editor) {
        if (editor.dataset.ready === "true") return;

        var surface = editor.querySelector("[data-editor-surface]");
        var inputId = editor.getAttribute("data-input");
        var input = inputId ? document.getElementById(inputId) : null;
        var toolbar = editor.querySelector(".editor-toolbar");
        var template = editor.querySelector("[data-editor-initial]");
        var status = editor.closest(".form-field")
            ? editor.closest(".form-field").querySelector("[data-editor-count]")
            : document.querySelector("[data-editor-count]");
        var imageInput = editor.querySelector("[data-editor-image-input]");
        var form = editor.closest("form");

        if (!surface || !input) return;

        var initialHtml = "";
        if (template) initialHtml = template.innerHTML;
        if (!initialHtml.trim()) initialHtml = input.value || "";
        if (/^(<p><br\s*\/?><\/p>|<br\s*\/?>|<p>\s*<\/p>)$/i.test(initialHtml.trim())) {
            initialHtml = "";
        }

        surface.innerHTML = initialHtml;
        input.hidden = true;
        editor.dataset.ready = "true";

        var savedRange = null;
        var uploading = false;
        var submitting = false;
        var initialSnapshot = surface.innerHTML;

        try {
            document.execCommand("styleWithCSS", false, false);
            document.execCommand("defaultParagraphSeparator", false, "p");
        } catch (e) { /* not supported in every browser */ }

        function sync() {
            var html = surface.innerHTML;
            if (/^(<p><br\s*\/?><\/p>|<br\s*\/?>|<p>\s*<\/p>)$/i.test(html.trim()) || !surface.textContent.trim()) {
                if (!surface.querySelector("img, hr, figure")) {
                    html = surface.innerHTML;
                }
            }

            input.value = html;

            if (status) {
                var plain = surface.innerText || "";
                var characters = plain.replace(/\s+/g, " ").trim().length;
                var words = plain.trim() ? plain.trim().split(/\s+/).length : 0;
                status.textContent = formatCount(words, characters);
            }
        }

        function markChanged() {
            sync();
            editor.classList.add("is-dirty");
        }

        function saveSelection() {
            var selection = window.getSelection();
            if (selection && selection.rangeCount > 0) {
                var range = selection.getRangeAt(0);
                if (surface.contains(range.commonAncestorContainer)) {
                    savedRange = range.cloneRange();
                }
            }
        }

        function restoreSelection() {
            surface.focus();
            if (!savedRange) return;

            var selection = window.getSelection();
            if (!selection) return;
            selection.removeAllRanges();
            selection.addRange(savedRange);
        }

        function run(command, value) {
            surface.focus();
            restoreSelection();
            try {
                document.execCommand(command, false, value || null);
            } catch (e) { /* ignore unsupported commands */ }
            markChanged();
            updateToolbarState();
        }

        function updateToolbarState() {
            if (!toolbar) return;

            var selection = window.getSelection();
            if (!selection || selection.rangeCount === 0 || !surface.contains(selection.anchorNode)) {
                return;
            }

            function active(command) {
                try {
                    return document.queryCommandState(command);
                } catch (e) {
                    return false;
                }
            }

            var block = "";
            try {
                block = (document.queryCommandValue("formatBlock") || "").toLowerCase();
            } catch (e) { /* ignore */ }

            Array.prototype.forEach.call(toolbar.querySelectorAll("[data-cmd]"), function (button) {
                var command = button.getAttribute("data-cmd");
                var value = (button.getAttribute("data-value") || "").toLowerCase();
                var isActive = false;

                if (command === "formatBlock") {
                    isActive = block === value || (value === "p" && (block === "div" || block === ""));
                } else if (["bold", "italic", "underline", "strikeThrough", "insertUnorderedList", "insertOrderedList"].indexOf(command) !== -1) {
                    isActive = active(command);
                }

                button.classList.toggle("is-active", isActive);
                button.setAttribute("aria-pressed", isActive ? "true" : "false");
            });
        }

        function insertImage(url, alt) {
            var html = '<figure><img src="' + escapeHtml(url) + '" alt="' + escapeHtml(alt || "") + '" /></figure><p><br /></p>';
            surface.focus();
            restoreSelection();
            try {
                document.execCommand("insertHTML", false, html);
            } catch (e) {
                surface.insertAdjacentHTML("beforeend", html);
            }
            markChanged();
        }

        function uploadImage(file) {
            if (!file || uploading) return;

            if (file.size > 5 * 1024 * 1024) {
                BayanUI.toast(isRtl ? "حجم الصورة يجب ألا يزيد عن 5 ميجابايت." : "Images must be 5 MB or smaller.", "error");
                return;
            }

            var body = new FormData();
            body.append("file", file);
            uploading = true;
            editor.classList.add("is-uploading");

            fetch("/api/uploads/image", {
                method: "POST",
                headers: {
                    "RequestVerificationToken": csrfToken,
                    "X-Requested-With": "XMLHttpRequest"
                },
                credentials: "same-origin",
                body: body
            }).then(function (response) {
                return response.json().then(function (data) {
                    if (!response.ok) throw new Error(data && data.message ? data.message : text("uploadError", "Upload failed."));
                    return data;
                });
            }).then(function (data) {
                if (!data || !data.url) throw new Error(text("uploadError", "Upload failed."));
                insertImage(data.url, text("imageAlt", "Article image"));
                BayanUI.toast(text("uploading", "Image added") === undefined ? "Image added" : (isRtl ? "تمت إضافة الصورة" : "Image added"));
            }).catch(function (error) {
                BayanUI.toast(error && error.message ? error.message : text("uploadError", "The image could not be uploaded."), "error");
            }).finally(function () {
                uploading = false;
                editor.classList.remove("is-uploading");
                if (imageInput) imageInput.value = "";
            });
        }

        function insertLink() {
            saveSelection();

            var selection = window.getSelection();
            var selectedText = selection && selection.rangeCount > 0 ? selection.toString().trim() : "";

            BayanUI.prompt({
                title: text("linkPrompt", "Insert link"),
                label: text("linkPrompt", "Paste the link address"),
                value: "",
                secondaryLabel: text("linkTextPrompt", "Text to display (optional)"),
                secondaryValue: selectedText
            }).then(function (result) {
                if (result === null) return;

                var url = normalizeUrl(result);
                if (!isSafeLink(url)) {
                    BayanUI.toast(isRtl ? "أدخل رابطًا صالحًا يبدأ بـ http أو https." : "Enter a valid link starting with http or https.", "error");
                    return;
                }

                restoreSelection();

                var secondaryField = document.getElementById("promptSecondary");
                var customText = secondaryField ? secondaryField.value.trim() : "";

                if (customText) {
                    var html = '<a href="' + escapeHtml(url) + '">' + escapeHtml(customText) + "</a>";
                    try {
                        document.execCommand("insertHTML", false, html);
                    } catch (e) {
                        surface.insertAdjacentHTML("beforeend", html);
                    }
                } else {
                    try {
                        document.execCommand("createLink", false, url);
                    } catch (e) { /* ignore */ }
                }

                markChanged();
            });
        }

        if (toolbar) {
            toolbar.addEventListener("mousedown", function (event) {
                if (event.target.closest("button")) event.preventDefault();
            });

            toolbar.addEventListener("click", function (event) {
                var button = event.target.closest("button");
                if (!button) return;

                var action = button.getAttribute("data-action");
                if (action === "image") {
                    saveSelection();
                    if (imageInput) imageInput.click();
                    return;
                }

                var command = button.getAttribute("data-cmd");
                if (!command) return;

                if (command === "createLink") {
                    insertLink();
                    return;
                }

                var value = button.getAttribute("data-value") || "";
                run(command, value);
            });
        }

        if (imageInput) {
            imageInput.addEventListener("change", function () {
                if (imageInput.files && imageInput.files[0]) {
                    uploadImage(imageInput.files[0]);
                }
            });
        }

        surface.addEventListener("input", markChanged);
        surface.addEventListener("blur", saveSelection);
        surface.addEventListener("keyup", updateToolbarState);
        surface.addEventListener("mouseup", function () {
            saveSelection();
            updateToolbarState();
        });

        surface.addEventListener("paste", function (event) {
            var clipboard = event.clipboardData;
            if (!clipboard) return;

            var textValue = clipboard.getData("text/plain");
            if (!textValue) return;

            event.preventDefault();

            var paragraphs = textValue.replace(/\r\n/g, "\n").replace(/\r/g, "\n").split(/\n{2,}/);
            var html = paragraphs.map(function (paragraph) {
                return "<p>" + escapeHtml(paragraph).replace(/\n/g, "<br />") + "</p>";
            }).join("");

            try {
                document.execCommand("insertHTML", false, html);
            } catch (e) {
                document.execCommand("insertText", false, textValue);
            }
            markChanged();
        });

        document.addEventListener("selectionchange", updateToolbarState);

        document.addEventListener("keydown", function (event) {
            if (!surface.contains(document.activeElement) && document.activeElement !== surface) return;

            if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === "k") {
                event.preventDefault();
                insertLink();
            }
        });

        if (form) {
            form.addEventListener("submit", function () {
                submitting = true;
                sync();
            });
        }

        window.addEventListener("beforeunload", function (event) {
            if (submitting) return;
            if (!editor.classList.contains("is-dirty")) return;
            if (surface.innerHTML === initialSnapshot) return;

            event.preventDefault();
            event.returnValue = text("unsavedText", "Your changes have not been saved yet.");
            return event.returnValue;
        });

        // Keep the fallback textarea in sync even when the form is submitted
        // through an unusual path (for example a keyboard shortcut).
        sync();
        updateToolbarState();
        editors.push(editor);
    }

    function boot() {
        Array.prototype.forEach.call(document.querySelectorAll("[data-editor]"), setup);

        // Expose a lightweight hook for debugging and future integrations.
        window.BayanEditor = {
            syncAll: function () {
                editors.forEach(function (editor) {
                    var input = document.getElementById(editor.getAttribute("data-input"));
                    var surface = editor.querySelector("[data-editor-surface]");
                    if (input && surface) input.value = surface.innerHTML;
                });
            }
        };
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", boot);
    } else {
        boot();
    }
})();
