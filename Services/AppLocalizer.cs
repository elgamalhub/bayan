using System.Globalization;

namespace Bayan.Services
{
    /// <summary>
    /// Lightweight, dependency-free string catalogue for the bilingual UI.
    /// Keeping the copy in one place avoids scattered literals and keeps the
    /// English and Arabic versions in sync.
    /// </summary>
    public static class AppStrings
    {
        private static readonly Dictionary<string, (string En, string Ar)> Values = new(StringComparer.OrdinalIgnoreCase)
        {
            // Shared
            ["common.save"] = ("Save changes", "حفظ التغييرات"),
            ["common.cancel"] = ("Cancel", "إلغاء"),
            ["common.delete"] = ("Delete", "حذف"),
            ["common.edit"] = ("Edit", "تعديل"),
            ["common.close"] = ("Close", "إغلاق"),
            ["common.confirm"] = ("Confirm", "تأكيد"),
            ["common.back"] = ("Back", "رجوع"),
            ["common.loading"] = ("Loading…", "جارٍ التحميل…"),
            ["common.error"] = ("Something went wrong", "حدث خطأ ما"),
            ["common.success"] = ("Done", "تم"),
            ["common.required"] = ("Required", "مطلوب"),
            ["common.optional"] = ("Optional", "اختياري"),
            ["common.search"] = ("Search", "بحث"),
            ["common.clear"] = ("Clear", "مسح"),
            ["common.all"] = ("All", "الكل"),
            ["common.viewAll"] = ("View all", "عرض الكل"),
            ["common.readMore"] = ("Read article", "اقرأ المقال"),
            ["common.minuteRead"] = ("{0} min read", "قراءة {0} د"),
            ["common.by"] = ("By", "بقلم"),
            ["common.share"] = ("Share", "مشاركة"),
            ["common.copied"] = ("Link copied", "تم نسخ الرابط"),
            ["common.copyLink"] = ("Copy link", "نسخ الرابط"),
            ["common.previous"] = ("Previous", "السابق"),
            ["common.next"] = ("Next", "التالي"),
            ["common.page"] = ("Page {0} of {1}", "صفحة {0} من {1}"),
            ["common.yes"] = ("Yes", "نعم"),
            ["common.no"] = ("No", "لا"),
            ["common.remove"] = ("Remove", "إزالة"),
            ["common.upload"] = ("Upload", "رفع"),
            ["common.chooseFile"] = ("Choose a file", "اختر ملفًا"),
            ["common.change"] = ("Change", "تغيير"),
            ["common.processing"] = ("Processing…", "جارٍ المعالجة…"),
            ["common.retry"] = ("Try again", "حاول مرة أخرى"),
            ["common.privacy"] = ("Privacy", "الخصوصية"),
            ["common.terms"] = ("Terms", "الشروط"),
            ["common.member"] = ("Member", "عضو"),
            ["common.copy"] = ("Copy", "نسخ"),
            ["common.skipToContent"] = ("Skip to content", "انتقل إلى المحتوى"),
            ["common.view"] = ("View", "عرض"),
            ["common.or"] = ("or", "أو"),
            ["common.secure"] = ("Private and secure", "خاص وآمن"),
            ["common.learnMore"] = ("Learn more", "اعرف المزيد"),

            // Navigation
            ["nav.home"] = ("Home", "الرئيسية"),
            ["nav.articles"] = ("Articles", "المقالات"),
            ["nav.categories"] = ("Topics", "المواضيع"),
            ["nav.write"] = ("Write", "اكتب"),
            ["nav.login"] = ("Sign in", "تسجيل الدخول"),
            ["nav.signup"] = ("Create account", "إنشاء حساب"),
            ["nav.profile"] = ("My profile", "ملفي الشخصي"),
            ["nav.myPosts"] = ("My articles", "مقالاتي"),
            ["nav.settings"] = ("Account settings", "إعدادات الحساب"),
            ["nav.logout"] = ("Sign out", "تسجيل الخروج"),
            ["nav.language"] = ("Language", "اللغة"),
            ["nav.theme"] = ("Toggle theme", "تبديل المظهر"),
            ["nav.menu"] = ("Menu", "القائمة"),
            ["nav.searchPlaceholder"] = ("Search articles…", "ابحث في المقالات…"),
            ["nav.brandTagline"] = ("Read. Think. Share.", "اقرأ. فكّر. شارك."),

            // Footer
            ["footer.tagline"] = ("An independent publication for people who like to read deeply, think clearly, and share what they learn.", "منصة مستقلة لمن يحبّون القراءة العميقة والتفكير الواضح ومشاركة ما يتعلّمونه."),
            ["footer.explore"] = ("Explore", "استكشف"),
            ["footer.community"] = ("Community", "المجتمع"),
            ["footer.allArticles"] = ("All articles", "كل المقالات"),
            ["footer.writeStory"] = ("Write an article", "اكتب مقالًا"),
            ["footer.about"] = ("About Bayan", "عن بيان"),
            ["footer.rights"] = ("All rights reserved.", "جميع الحقوق محفوظة."),
            ["footer.note"] = ("Built for readers and writers.", "صُنعت للقرّاء والكتّاب."),

            // Home
            ["home.eyebrow"] = ("Independent ideas, thoughtfully written", "أفكار مستقلة، مكتوبة بعناية"),
            ["home.title1"] = ("Read deeply.", "اقرأ بعمق."),
            ["home.titleAccent"] = ("Think clearly.", "فكّر بوضوح."),
            ["home.title2"] = ("Share what you learn.", "شارك ما تتعلّمه."),
            ["home.subtitle"] = ("Bayan is a calm space for essays, practical guides, and honest notes on learning, technology, work, and life.", "بيان مساحة هادئة للمقالات والأدلة العملية والملاحظات الصادقة عن التعلّم والتقنية والعمل والحياة."),
            ["home.cta"] = ("Start reading", "ابدأ القراءة"),
            ["home.ctaSecondary"] = ("Write an article", "اكتب مقالًا"),
            ["home.searchPlaceholder"] = ("What would you like to read about?", "عن ماذا تودّ أن تقرأ؟"),
            ["home.statArticles"] = ("Published articles", "مقال منشور"),
            ["home.statWriters"] = ("Writers", "كاتب"),
            ["home.statCategories"] = ("Topics", "موضوع"),
            ["home.latestEyebrow"] = ("Fresh from the desk", "جديد المنصة"),
            ["home.latestTitle"] = ("Latest articles", "أحدث المقالات"),
            ["home.latestSubtitle"] = ("New perspectives, carefully edited and easy to read.", "أفكار جديدة، محرّرة بعناية وسهلة القراءة."),
            ["home.featured"] = ("Editor's pick", "اختيار المحرر"),
            ["home.popularEyebrow"] = ("Most appreciated", "الأكثر إعجابًا"),
            ["home.popularTitle"] = ("Popular this month", "الأكثر قراءة هذا الشهر"),
            ["home.categoriesEyebrow"] = ("Find your corner", "اعثر على ما يناسبك"),
            ["home.categoriesTitle"] = ("Browse by topic", "تصفّح حسب الموضوع"),
            ["home.categoriesSubtitle"] = ("Start with a subject that sparks your curiosity.", "ابدأ بموضوع يثير فضولك."),
            ["home.whyEyebrow"] = ("Why Bayan", "لماذا بيان"),
            ["home.whyTitle"] = ("A better place to read and write online", "مكان أفضل للقراءة والكتابة"),
            ["home.whySubtitle"] = ("No noise, no endless feeds. Just clear writing and a community that values thoughtful ideas.", "بلا ضجيج أو تدفقات لا تنتهي. فقط كتابة واضحة ومجتمع يقدّر الأفكار المتأنية."),
            ["home.why1Title"] = ("Writing worth your time", "كتابة تستحق وقتك"),
            ["home.why1Text"] = ("Every article is edited for clarity, structure, and usefulness before it reaches the homepage.", "كل مقال يُحرّر من أجل الوضوح والبنية والفائدة قبل أن يصل إلى الصفحة الرئيسية."),
            ["home.why2Title"] = ("A quiet, focused reader", "تجربة قراءة هادئة ومركّزة"),
            ["home.why2Text"] = ("A clean reading experience with no pop-ups, no autoplay, and no distractions.", "تجربة قراءة نظيفة بلا نوافذ منبثقة أو تشغيل تلقائي أو مشتتات."),
            ["home.why3Title"] = ("Built for thoughtful exchange", "مصمّمة للنقاش الهادف"),
            ["home.why3Text"] = ("Follow the topics you care about and join the conversation with comments that add value.", "تابع المواضيع التي تهمّك وشارك في النقاش بتعليقات تضيف قيمة."),
            ["home.ctaEyebrow"] = ("Your turn", "دورك الآن"),
            ["home.ctaTitle"] = ("Have something worth saying?", "لديك ما يستحق أن يُقال؟"),
            ["home.ctaText"] = ("Share your experience, a practical guide, or a lesson you learned. Your first article starts with a blank page.", "شارك تجربتك أو دليلًا عمليًا أو درسًا تعلّمته. مقالك الأول يبدأ بصفحة بيضاء."),
            ["home.ctaButton"] = ("Write on Bayan", "اكتب على بيان"),
            ["home.empty"] = ("The first article is waiting to be written.", "المقال الأول في انتظار من يكتبه."),
            ["home.readFeatured"] = ("Read the editor's pick", "اقرأ اختيار المحرر"),

            // Blog index
            ["blog.title"] = ("Articles", "المقالات"),
            ["blog.subtitle"] = ("Essays, guides, and notes on the things worth learning.", "مقالات وأدلة وملاحظات عن الأشياء التي تستحق التعلّم."),
            ["blog.searchPlaceholder"] = ("Search by title or content…", "ابحث بالعنوان أو المحتوى…"),
            ["blog.filter"] = ("Filter", "تصفية"),
            ["blog.allCategories"] = ("All topics", "كل المواضيع"),
            ["blog.sortNewest"] = ("Newest first", "الأحدث أولًا"),
            ["blog.sortOldest"] = ("Oldest first", "الأقدم أولًا"),
            ["blog.sortLiked"] = ("Most liked", "الأكثر إعجابًا"),
            ["blog.results"] = ("{0} articles", "{0} مقال"),
            ["blog.resultOne"] = ("1 article", "مقال واحد"),
            ["blog.noResults"] = ("No articles found", "لا توجد مقالات"),
            ["blog.noResultsText"] = ("Try a different search term or explore another topic.", "جرّب كلمة بحث أخرى أو استكشف موضوعًا آخر."),
            ["blog.clearFilters"] = ("Clear filters", "مسح عوامل التصفية"),
            ["blog.popular"] = ("Popular articles", "مقالات شائعة"),
            ["blog.popularSubtitle"] = ("What readers are appreciating right now.", "ما يقدّره القرّاء الآن."),
            ["blog.categories"] = ("Topics", "المواضيع"),
            ["blog.writeCtaTitle"] = ("Share your perspective", "شارك وجهة نظرك"),
            ["blog.writeCtaText"] = ("Good writing starts with a clear idea. Publish yours in a calm, ad-free space.", "الكتابة الجيدة تبدأ بفكرة واضحة. انشر فكرتك في مساحة هادئة بلا إعلانات."),
            ["blog.writeCtaButton"] = ("Start writing", "ابدأ الكتابة"),
            ["blog.searchLabel"] = ("Search articles", "البحث في المقالات"),

            // Post
            ["post.published"] = ("Published {0}", "نُشر في {0}"),
            ["post.updated"] = ("Updated {0}", "حُدّث في {0}"),
            ["post.like"] = ("Like", "إعجاب"),
            ["post.liked"] = ("Liked", "أعجبني"),
            ["post.likeAria"] = ("Like this article", "أعجبني هذا المقال"),
            ["post.commentsTitle"] = ("Comments", "التعليقات"),
            ["post.commentsSubtitle"] = ("Join the conversation. Keep it kind and useful.", "شارك في النقاش. اجعله لطيفًا ومفيدًا."),
            ["post.noComments"] = ("No comments yet. Start the conversation.", "لا توجد تعليقات بعد. ابدأ النقاش."),
            ["post.leaveComment"] = ("Leave a comment", "اترك تعليقًا"),
            ["post.commentPlaceholder"] = ("Add something meaningful to the discussion…", "أضف شيئًا مفيدًا إلى النقاش…"),
            ["post.postComment"] = ("Post comment", "نشر التعليق"),
            ["post.loginToComment"] = ("Sign in", "سجّل الدخول"),
            ["post.loginToCommentText"] = ("to join the conversation.", "للمشاركة في النقاش."),
            ["post.deleteTitle"] = ("Delete this article?", "حذف هذا المقال؟"),
            ["post.deleteText"] = ("This action cannot be undone. The article, its comments, and its likes will be permanently removed.", "لا يمكن التراجع عن هذا الإجراء. سيُحذف المقال وتعليقاته وإعجاباته نهائيًا."),
            ["post.deleteConfirm"] = ("Delete article", "حذف المقال"),
            ["post.deleteCancel"] = ("Keep article", "الاحتفاظ بالمقال"),
            ["comment.deleteTitle"] = ("Delete this comment?", "حذف هذا التعليق؟"),
            ["comment.deleteText"] = ("This action cannot be undone. The comment will be permanently removed from the discussion.", "لا يمكن التراجع عن هذا الإجراء. سيُحذف التعليق نهائيًا من النقاش."),
            ["comment.deleteConfirm"] = ("Delete comment", "حذف التعليق"),
            ["comment.deleteCancel"] = ("Keep comment", "الاحتفاظ بالتعليق"),
            ["post.authorAbout"] = ("About the author", "عن الكاتب"),
            ["post.authorPosts"] = ("Published articles", "المقالات المنشورة"),
            ["post.joined"] = ("Joined {0}", "انضم في {0}"),
            ["post.related"] = ("Keep reading", "تابع القراءة"),
            ["post.relatedSubtitle"] = ("More from the same topic.", "المزيد من الموضوع نفسه."),
            ["post.share"] = ("Share this article", "شارك هذا المقال"),
            ["post.edited"] = ("Edited", "مُعدّل"),
            ["post.minRead"] = ("{0} min read", "{0} دقيقة قراءة"),
            ["post.category"] = ("Topic", "الموضوع"),

            // Editor
            ["editor.createTitle"] = ("Write a new article", "اكتب مقالًا جديدًا"),
            ["editor.createSubtitle"] = ("A clear idea, a useful structure, and your own voice. That is all you need.", "فكرة واضحة وبنية مفيدة وصوتك الخاص. هذا كل ما تحتاجه."),
            ["editor.editTitle"] = ("Edit article", "تعديل المقال"),
            ["editor.editSubtitle"] = ("Refine the writing, update the details, or replace the cover.", "حسّن الكتابة أو حدّث التفاصيل أو استبدل صورة الغلاف."),
            ["editor.titleLabel"] = ("Article title", "عنوان المقال"),
            ["editor.titlePlaceholder"] = ("A clear, specific title", "عنوان واضح ومحدّد"),
            ["editor.categoryLabel"] = ("Topic", "الموضوع"),
            ["editor.categoryPlaceholder"] = ("Choose a topic", "اختر موضوعًا"),
            ["editor.coverLabel"] = ("Cover image", "صورة الغلاف"),
            ["editor.coverHint"] = ("JPG, PNG, GIF or WebP. Up to 5 MB. Recommended size 1600×900.", "JPG أو PNG أو GIF أو WebP. حتى 5 ميجابايت. المقاس المفضل 1600×900."),
            ["editor.coverChoose"] = ("Choose cover", "اختر غلافًا"),
            ["editor.coverRemove"] = ("Remove cover", "إزالة الغلاف"),
            ["editor.contentLabel"] = ("Article content", "محتوى المقال"),
            ["editor.contentPlaceholder"] = ("Start writing your article…", "ابدأ بكتابة مقالك…"),
            ["editor.publish"] = ("Publish article", "نشر المقال"),
            ["editor.saveChanges"] = ("Save changes", "حفظ التغييرات"),
            ["editor.cancel"] = ("Cancel", "إلغاء"),
            ["editor.preview"] = ("Preview", "معاينة"),
            ["editor.guidelinesTitle"] = ("Publishing guidelines", "إرشادات النشر"),
            ["editor.guideline1"] = ("Write for a curious reader, not for a search engine.", "اكتب لقرّاء فضوليين، لا لمحركات البحث."),
            ["editor.guideline2"] = ("Use short paragraphs and descriptive headings.", "استخدم فقرات قصيرة وعناوين واضحة."),
            ["editor.guideline3"] = ("Credit sources and avoid copying others' work.", "اذكر المصادر وتجنّب نسخ أعمال الآخرين."),
            ["editor.guideline4"] = ("Keep the title specific and honest.", "اجعل العنوان محدّدًا وصادقًا."),
            ["editor.tipsTitle"] = ("Before you publish", "قبل النشر"),
            ["editor.tip1"] = ("Read your article aloud once.", "اقرأ مقالك بصوت عالٍ مرة واحدة."),
            ["editor.tip2"] = ("Check that every section adds something new.", "تأكد أن كل قسم يضيف شيئًا جديدًا."),
            ["editor.tip3"] = ("Add a cover that matches the tone of the piece.", "أضف غلافًا يناسب نبرة المقال."),
            ["editor.wordCount"] = ("{0} words", "{0} كلمة"),
            ["editor.charCount"] = ("{0} characters", "{0} حرفًا"),
            ["editor.unsavedTitle"] = ("Leave without saving?", "المغادرة دون حفظ؟"),
            ["editor.unsavedText"] = ("Your changes have not been saved yet.", "لم تُحفظ تغييراتك بعد."),
            ["editor.leave"] = ("Leave page", "مغادرة الصفحة"),
            ["editor.stay"] = ("Keep editing", "متابعة التحرير"),
            ["editor.minChars"] = ("Write at least 50 characters of readable content.", "اكتب 50 حرفًا على الأقل من المحتوى المقروء."),
            ["editor.categoryRequired"] = ("Choose a topic for your article.", "اختر موضوعًا لمقالك."),
            ["editor.uploading"] = ("Uploading image…", "جارٍ رفع الصورة…"),
            ["editor.uploadError"] = ("The image could not be uploaded. Please try again.", "تعذّر رفع الصورة. حاول مرة أخرى."),
            ["editor.imageAlt"] = ("Article image", "صورة المقال"),
            ["editor.toolbar.bold"] = ("Bold", "غامق"),
            ["editor.toolbar.italic"] = ("Italic", "مائل"),
            ["editor.toolbar.underline"] = ("Underline", "تحته خط"),
            ["editor.toolbar.strike"] = ("Strikethrough", "يتوسطه خط"),
            ["editor.toolbar.h2"] = ("Heading", "عنوان رئيسي"),
            ["editor.toolbar.h3"] = ("Subheading", "عنوان فرعي"),
            ["editor.toolbar.paragraph"] = ("Paragraph", "فقرة"),
            ["editor.toolbar.bulletList"] = ("Bullet list", "قائمة نقطية"),
            ["editor.toolbar.numberedList"] = ("Numbered list", "قائمة مرقّمة"),
            ["editor.toolbar.quote"] = ("Quote", "اقتباس"),
            ["editor.toolbar.code"] = ("Code block", "كتلة برمجية"),
            ["editor.toolbar.link"] = ("Insert link", "إدراج رابط"),
            ["editor.toolbar.image"] = ("Insert image", "إدراج صورة"),
            ["editor.toolbar.undo"] = ("Undo", "تراجع"),
            ["editor.toolbar.redo"] = ("Redo", "إعادة"),
            ["editor.toolbar.clear"] = ("Clear formatting", "مسح التنسيق"),
            ["editor.linkPrompt"] = ("Paste the link address", "الصق عنوان الرابط"),
            ["editor.linkTextPrompt"] = ("Text to display (optional)", "النص الظاهر (اختياري)"),
            ["editor.insert"] = ("Insert", "إدراج"),
            ["editor.imageBadge"] = ("Insert image", "إدراج صورة"),
            ["editor.editImageAlt"] = ("Describe the image", "صف الصورة"),
            ["editor.saved"] = ("Saved", "تم الحفظ"),

            // Auth
            ["auth.loginTitle"] = ("Welcome back", "مرحبًا بعودتك"),
            ["auth.loginSubtitle"] = ("Sign in to write, comment, and keep track of your articles.", "سجّل الدخول لتكتب وتعلّق وتتابع مقالاتك."),
            ["auth.signupTitle"] = ("Create your account", "أنشئ حسابك"),
            ["auth.signupSubtitle"] = ("Join a community that values clear thinking and good writing.", "انضم إلى مجتمع يقدّر التفكير الواضح والكتابة الجيدة."),
            ["auth.username"] = ("Username", "اسم المستخدم"),
            ["auth.email"] = ("Email address", "البريد الإلكتروني"),
            ["auth.password"] = ("Password", "كلمة المرور"),
            ["auth.confirmPassword"] = ("Confirm password", "تأكيد كلمة المرور"),
            ["auth.remember"] = ("Remember me", "تذكّرني"),
            ["auth.loginButton"] = ("Sign in", "تسجيل الدخول"),
            ["auth.signupButton"] = ("Create account", "إنشاء الحساب"),
            ["auth.noAccount"] = ("New to Bayan?", "جديد على بيان؟"),
            ["auth.hasAccount"] = ("Already have an account?", "لديك حساب بالفعل؟"),
            ["auth.panelTitle"] = ("A quiet place for good ideas.", "مساحة هادئة للأفكار الجيدة."),
            ["auth.panelText"] = ("Read considered articles, follow the topics you care about, and publish your own work in a space built for focus.", "اقرأ مقالات متأنية، وتابع المواضيع التي تهمّك، وانشر أعمالك في مساحة مصممة للتركيز."),
            ["auth.panelQuote"] = ("“The more that you read, the more things you will know.”", "«كلما قرأت أكثر، عرفت أكثر.»"),
            ["auth.panelQuoteAuthor"] = ("Dr. Seuss", "د. سوس"),
            ["auth.passwordHint"] = ("At least 6 characters.", "6 أحرف على الأقل."),
            ["auth.welcome"] = ("Welcome back, {0}.", "مرحبًا بعودتك، {0}."),
            ["auth.registered"] = ("Your account is ready. Sign in to continue.", "حسابك جاهز. سجّل الدخول للمتابعة."),
            ["auth.invalid"] = ("The username or password is incorrect.", "اسم المستخدم أو كلمة المرور غير صحيحة."),
            ["auth.locked"] = ("Too many attempts. Please try again later.", "محاولات كثيرة. حاول مرة أخرى لاحقًا."),
            ["auth.duplicateUser"] = ("That username is already taken.", "اسم المستخدم مستخدم بالفعل."),
            ["auth.duplicateEmail"] = ("An account with that email already exists.", "يوجد حساب بهذا البريد الإلكتروني بالفعل."),
            ["auth.returnUrl"] = ("Continue", "متابعة"),
            ["auth.showPassword"] = ("Show password", "إظهار كلمة المرور"),
            ["auth.hidePassword"] = ("Hide password", "إخفاء كلمة المرور"),
            ["auth.loginLink"] = ("Sign in", "تسجيل الدخول"),
            ["auth.signupLink"] = ("Create one", "أنشئ حسابًا"),

            // Profile
            ["profile.title"] = ("Profile", "الملف الشخصي"),
            ["profile.editTitle"] = ("Account settings", "إعدادات الحساب"),
            ["profile.editSubtitle"] = ("Update your public profile and personal details.", "حدّث ملفك العام وبياناتك الشخصية."),
            ["profile.username"] = ("Username", "اسم المستخدم"),
            ["profile.email"] = ("Email address", "البريد الإلكتروني"),
            ["profile.emailHint"] = ("Your email is private and never shown publicly.", "بريدك الإلكتروني خاص ولا يظهر للعامة أبدًا."),
            ["profile.bio"] = ("Short bio", "نبذة مختصرة"),
            ["profile.bioPlaceholder"] = ("A sentence or two about you and what you write about.", "جملة أو جملتان عنك وعن ما تكتب عنه."),
            ["profile.linkedin"] = ("LinkedIn", "لينكدإن"),
            ["profile.website"] = ("Website", "الموقع الإلكتروني"),
            ["profile.picture"] = ("Profile photo", "الصورة الشخصية"),
            ["profile.pictureHint"] = ("Square image, up to 2 MB.", "صورة مربعة، حتى 2 ميجابايت."),
            ["profile.save"] = ("Save profile", "حفظ الملف"),
            ["profile.memberSince"] = ("Member since {0}", "عضو منذ {0}"),
            ["profile.posts"] = ("Articles", "المقالات"),
            ["profile.noPosts"] = ("No published articles yet.", "لا توجد مقالات منشورة بعد."),
            ["profile.editProfile"] = ("Edit profile", "تعديل الملف"),
            ["profile.writePost"] = ("Write an article", "اكتب مقالًا"),
            ["profile.about"] = ("About", "نبذة"),
            ["profile.statsPosts"] = ("Articles", "مقالات"),
            ["profile.statsLikes"] = ("Likes", "إعجابات"),
            ["profile.statsComments"] = ("Comments", "تعليقات"),
            ["profile.updated"] = ("Your profile has been updated.", "تم تحديث ملفك الشخصي."),
            ["profile.usernameTaken"] = ("That username is already in use.", "اسم المستخدم مستخدم بالفعل."),
            ["profile.emailTaken"] = ("That email is already in use.", "البريد الإلكتروني مستخدم بالفعل."),
            ["profile.imageInvalid"] = ("Please choose a valid image file.", "اختر ملف صورة صالحًا."),
            ["profile.viewProfile"] = ("View public profile", "عرض الملف العام"),
            ["profile.privateNote"] = ("Your email is only visible to you.", "بريدك الإلكتروني مرئي لك فقط."),
            ["profile.fullName"] = ("Full name", "الاسم الكامل"),
            ["profile.fullNameHint"] = ("Shown on your articles and profile.", "يظهر على مقالاتك وملفك."),
            ["profile.fullNamePlaceholder"] = ("Your name as readers will see it", "اسمك كما سيراه القرّاء"),
            ["profile.links"] = ("Links", "الروابط"),
            ["profile.linksHint"] = ("Add a website or LinkedIn profile. Links must start with http:// or https://.", "أضف موقعًا أو ملفًا على لينكدإن. يجب أن تبدأ الروابط بـ http:// أو https://."),
            ["profile.currentPhoto"] = ("Current photo", "الصورة الحالية"),
            ["profile.changePhoto"] = ("Change photo", "تغيير الصورة"),
            ["profile.noBio"] = ("This writer has not added a bio yet.", "لم يضف هذا الكاتب نبذة بعد."),
            ["profile.postsBy"] = ("Articles by {0}", "مقالات {0}"),
            ["profile.previewNote"] = ("This is how readers see your profile.", "هكذا يرى القرّاء ملفك الشخصي."),
            ["profile.tipsTitle"] = ("A complete profile helps readers trust your work.", "الملف المكتمل يساعد القرّاء على الثقة بعملك."),
            ["profile.tip1"] = ("Add a clear photo of yourself.", "أضف صورة واضحة لنفسك."),
            ["profile.tip2"] = ("Write a short bio in your own voice.", "اكتب نبذة قصيرة بأسلوبك."),
            ["profile.tip3"] = ("Link to your work or professional profile.", "أضف رابطًا لعملك أو ملفك المهني."),

            // My articles
            ["my.title"] = ("My articles", "مقالاتي"),
            ["my.subtitle"] = ("Everything you have published, in one place.", "كل ما نشرته في مكان واحد."),
            ["my.empty"] = ("You have not published anything yet.", "لم تنشر شيئًا بعد."),
            ["my.emptyText"] = ("Your first article is a blank page and a good idea. Start whenever you are ready.", "مقالك الأول صفحة بيضاء وفكرة جيدة. ابدأ متى شئت."),
            ["my.write"] = ("Write an article", "اكتب مقالًا"),
            ["my.published"] = ("Published", "منشور"),
            ["my.updated"] = ("Updated {0}", "حُدّث {0}"),
            ["my.view"] = ("View", "عرض"),
            ["my.delete"] = ("Delete", "حذف"),
            ["my.edit"] = ("Edit", "تعديل"),

            // Deletion
            ["delete.subtitle"] = ("Please review before continuing.", "يرجى المراجعة قبل المتابعة."),
            ["delete.warning"] = ("This permanently removes the article and all of its comments and likes. This cannot be undone.", "سيُحذف المقال وكل تعليقاته وإعجاباته نهائيًا. لا يمكن التراجع عن ذلك."),
            ["delete.article"] = ("Article", "المقال"),
            ["delete.comments"] = ("Comments", "التعليقات"),
            ["delete.likes"] = ("Likes", "الإعجابات"),
            ["delete.keep"] = ("Keep article", "الاحتفاظ بالمقال"),

            // Language
            ["language.english"] = ("English", "الإنجليزية"),
            ["language.arabic"] = ("Arabic", "العربية"),
            ["language.switch"] = ("Change language", "تغيير اللغة"),

            // Errors
            ["error.title"] = ("Something unexpected happened", "حدث شيء غير متوقع"),
            ["error.text"] = ("We could not complete your request. Please try again, or return to the homepage.", "تعذّر إكمال طلبك. حاول مرة أخرى أو عُد إلى الصفحة الرئيسية."),
            ["error.home"] = ("Back to homepage", "العودة إلى الرئيسية"),
            ["error.requestId"] = ("Reference", "المرجع"),
            ["notfound.title"] = ("Page not found", "الصفحة غير موجودة"),
            ["notfound.text"] = ("The page you are looking for may have moved, or the link is incorrect.", "ربما انتقلت الصفحة التي تبحث عنها أو أن الرابط غير صحيح."),

            // Privacy
            ["privacy.title"] = ("Privacy policy", "سياسة الخصوصية"),
            ["privacy.updated"] = ("Last updated: January 2026", "آخر تحديث: يناير 2026"),
            ["privacy.intro"] = ("Bayan is built around a simple principle: your data belongs to you. This page explains what we collect and why.", "بُنيت بيان على مبدأ بسيط: بياناتك ملك لك. توضح هذه الصفحة ما نجمعه ولماذا."),
            ["privacy.section1Title"] = ("What we collect", "ما الذي نجمعه"),
            ["privacy.section1Text"] = ("When you create an account we store your username, email address, and password hash. When you publish, we store your articles, comments, and likes. We do not use advertising trackers.", "عند إنشاء حساب نحفظ اسم المستخدم والبريد الإلكتروني وتجزئة كلمة المرور. وعند النشر نحفظ مقالاتك وتعليقاتك وإعجاباتك. لا نستخدم أدوات تتبّع إعلانية."),
            ["privacy.section2Title"] = ("How we use it", "كيف نستخدمها"),
            ["privacy.section2Text"] = ("Your data is used only to operate the platform: signing you in, showing your articles, and enabling comments and likes. Your email address is never displayed publicly.", "تُستخدم بياناتك فقط لتشغيل المنصة: تسجيل دخولك وعرض مقالاتك وتمكين التعليقات والإعجابات. لا يظهر بريدك الإلكتروني للعامة أبدًا."),
            ["privacy.section3Title"] = ("Your choices", "خياراتك"),
            ["privacy.section3Text"] = ("You can edit or remove your profile information at any time. Deleting an article removes its comments and likes as well. Contact the site administrator if you need your account removed.", "يمكنك تعديل معلومات ملفك أو إزالتها في أي وقت. حذف المقال يزيل تعليقاته وإعجاباته أيضًا. تواصل مع مسؤول الموقع إذا كنت بحاجة إلى حذف حسابك."),
            ["privacy.contactTitle"] = ("Questions", "أسئلة"),
            ["privacy.contactText"] = ("If you have a question about this policy, contact the site administrator through the profile page.", "إذا كان لديك سؤال عن هذه السياسة، تواصل مع مسؤول الموقع عبر صفحة الملف الشخصي."),

            // Toasts
            ["toast.loginRequired"] = ("Sign in required", "تسجيل الدخول مطلوب"),
            ["toast.loginRequiredText"] = ("Sign in to like articles and join the conversation.", "سجّل الدخول للإعجاب بالمقالات والمشاركة في النقاش."),
            ["toast.imageUploaded"] = ("Image added", "تمت إضافة الصورة"),
            ["toast.error"] = ("Something went wrong. Please try again.", "حدث خطأ ما. حاول مرة أخرى."),
            ["toast.likeError"] = ("Could not update your like. Please try again.", "تعذّر تحديث الإعجاب. حاول مرة أخرى."),
            ["toast.commentAdded"] = ("Comment posted.", "تم نشر التعليق."),
            ["toast.commentUpdated"] = ("Comment updated.", "تم تحديث التعليق."),
            ["toast.commentDeleted"] = ("Comment deleted.", "تم حذف التعليق."),
            ["toast.articlePublished"] = ("Your article is live.", "تم نشر مقالك."),
            ["toast.articleUpdated"] = ("Your changes are saved.", "تم حفظ تغييراتك."),
            ["toast.articleDeleted"] = ("The article has been deleted.", "تم حذف المقال."),

            // Validation
            ["val.required"] = ("This field is required.", "هذا الحقل مطلوب."),
            ["val.email"] = ("Enter a valid email address.", "أدخل بريدًا إلكترونيًا صالحًا."),
            ["val.minLength"] = ("This field must be at least {1} characters.", "يجب ألا يقل هذا الحقل عن {1} حرفًا."),
            ["val.maxLength"] = ("This field must be at most {1} characters.", "يجب ألا يزيد هذا الحقل عن {1} حرفًا."),
            ["val.compare"] = ("The passwords do not match.", "كلمتا المرور غير متطابقتين."),
            ["val.username"] = ("Use 3–32 letters, numbers, dots, dashes or underscores.", "استخدم من 3 إلى 32 حرفًا أو رقمًا أو نقطة أو شرطة."),
            ["val.url"] = ("Enter a valid web address.", "أدخل عنوان موقع صالحًا."),
            ["val.contentMin"] = ("The article needs at least 50 characters of readable text.", "يحتاج المقال إلى 50 حرفًا على الأقل من النص المقروء."),
            ["val.commentRequired"] = ("Write a comment before posting.", "اكتب تعليقًا قبل النشر."),
            ["val.commentMax"] = ("Comments can be at most 1000 characters.", "يجب ألا يزيد التعليق عن 1000 حرف."),
            ["val.titleMax"] = ("Titles can be at most 100 characters.", "يجب ألا يزيد العنوان عن 100 حرف.")
        };

        public static bool IsArabic =>
            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("ar", StringComparison.OrdinalIgnoreCase);

        public static string Get(string key)
        {
            if (!Values.TryGetValue(key, out var value))
                return key;

            return IsArabic ? value.Ar : value.En;
        }

        public static string Format(string key, params object[] args)
        {
            var template = Get(key);
            try
            {
                return string.Format(CultureInfo.CurrentCulture, template, args);
            }
            catch (FormatException)
            {
                return template;
            }
        }
    }

    /// <summary>View-friendly wrapper around <see cref="AppStrings"/>.</summary>
    public class AppLocalizer
    {
        public string this[string key] => AppStrings.Get(key);

        public bool IsRtl => AppStrings.IsArabic;

        public string Dir => IsRtl ? "rtl" : "ltr";

        public string Culture => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        public string Format(string key, params object[] args) => AppStrings.Format(key, args);

        public string Date(DateTime value) =>
            value.Year < 1900
                ? "—"
                : value.ToString("MMM d, yyyy", CultureInfo.CurrentCulture);

        public string ShortDate(DateTime value) =>
            value.Year < 1900
                ? "—"
                : value.ToString("MMM d", CultureInfo.CurrentCulture);

        public string Number(int value) =>
            value.ToString("N0", CultureInfo.CurrentCulture);
    }
}
