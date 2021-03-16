function toggle_forgotten() {
    var element = document.getElementById("login-display");
    element.classList.toggle("hidden");
    var element2 = document.getElementById("forgotten-pass-display");
    element2.classList.toggle("hidden");
}

function toggle_create() {
    var element = document.getElementById("create-panel");
    element.classList.toggle("hidden");
    var element2 = document.getElementById("com-select-panel");
    element2.classList.toggle("hidden");
}


var rightMenuItems = document.querySelectorAll('div[id^=right-bar-main]');


// get any community checkbox selection on change
for (var i = 0; i < rightMenuItems.length; i++) {
    rightMenuItems[i].addEventListener("mouseenter", function (event) {
        var j = this.id.lastIndexOf('-');
        var n = this.id.substring(j + 1);
        toggle_width(n, 0);
    });
    rightMenuItems[i].addEventListener("mouseleave", function (event) {
        var j = this.id.lastIndexOf('-');
        var n = this.id.substring(j + 1);
        toggle_width(n, 1);
    });
}

function toggle_width(n, i) {
    var element = document.getElementById("right-bar-pic-cont-" + n);
    var main_element = document.getElementById("right-bar-text-cont-" + n);
    if (i == 0) {
        element.classList.remove('col-lg-12');
        element.classList.add('col-lg-6');
        main_element.classList.add('show');
    }

    if (i == 1) {
        element.classList.remove('col-lg-6');
        element.classList.add('col-lg-12');
        main_element.classList.remove('show');
    }

}





document.addEventListener("DOMContentLoaded", function (event) {

    

    setTimeout(function () {
        var loading = document.getElementById("page-loader");
        document.getElementById('tempbody').classList.remove("blurbody");
        loading.classList.add("hidden");
    }, 1000 + Math.floor(Math.random() * 800));



    if (window.location.href.indexOf("forgotten") > -1) {
        toggle_forgotten();
    }

    var allCheckboxes = document.querySelectorAll('input[id^=Community-Check]');


    // get any community checkbox selection on change
    for (var i = 0; i < allCheckboxes.length; i++) {
        allCheckboxes[i].addEventListener("change", function (event) {
            var j = this.id.lastIndexOf('-')
            var n = this.id.substring(j + 1);

            ChangeCheckboxCSS(n)
        })
    }

    function ChangeCheckboxCSS(n) {
        var element = document.getElementById("community-label-" + n);
        element.classList.toggle("checked");
    }

    var searchInput = document.getElementById('small-search');

    searchInput.addEventListener("keyup", function (event) {
        if (event.keyCode === 13) {
            // stop default
            searchActive();
            search();
        }
    });

});


function openChat() {
    document.getElementById("mychatMenu").style.width = "100%";
}

function closeChat() {
    document.getElementById("mychatMenu").style.width = "0";
}

function openSearch() {
    document.getElementById("mysearchMenu").style.width = "100%";
    loop();
}

function closeSearch() {
    document.getElementById("mysearchMenu").style.width = "0";
}

setTimeout(setConnectionID, 100);
setTimeout(setConnectionID, 1500);
setTimeout(updateShards, 100);
function getSessionID() {
    var name = "SessionID=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function loop() {
    search();
    setTimeout(loop, 200);
}

function redirectLogin() {
    window.location.href = "/Login";
}

function redirectLogout() {
    window.location.href = "/Logout";
}

function redirectCreateAccount() {
    window.location.href = "/CreateAccount";
}

var $active_menu;

function searchActive() {
    document.getElementById("overlay-toggle").checked = true;
    var n = 1;
    var siteOverlay = document.getElementById('site-overlay');
    siteOverlay.classList.add("active-" + n);
    $active_menu = n;
    var searchValue = document.getElementById("small-search").value;
    document.getElementById("searchbarmain").value = searchValue;
    document.getElementById("small-search").value = '';
}

// checks if an overlay is active and activates it
function menuActive(n) {
    if (getSessionID() != null && getSessionID() != "") {
        if ($active_menu == null) {
            document.getElementById("overlay-toggle").checked = true;
            var siteOverlay = document.getElementById('site-overlay');
            siteOverlay.classList.add("active-" + n);
            $active_menu = n;
        } else {
            document.getElementById("overlay-toggle").checked = true;
            var siteOverlay = document.getElementById('site-overlay');
            siteOverlay.classList.remove("active-" + $active_menu);
            $active_menu = null;
            var siteOverlay = document.getElementById('site-overlay');
            siteOverlay.classList.add("active-" + n);
            $active_menu = n;
        }
    } else {
        redirectLogin();
    }
}

window.toggleLike = toggleLike;
function toggleLike(id) {
    if (getSessionID() != null && getSessionID() != "") {
        const element = document.querySelector("#like-btn-" + id);
        if (element.classList.contains("active")) {
            element.classList.remove("active");
        } else {
            element.classList.add("active");
        }
    } else {
        window.location.href = "/Login";
    }
}

window.toggleSubscribe = toggleSubscribe;
function toggleSubscribe(id) {
    if (getSessionID() != null && getSessionID() != "") {
        const element = document.querySelector("#sub-btn");
        if (element.classList.contains("SUBSCRIBED")) {
            element.classList.remove("SUBSCRIBED");
        } else {
            element.classList.add("SUBSCRIBED");
        }
    } else {
        window.location.href = "/Login";
    }
}

function toggleMod(id) {
    if (getSessionID() != null && getSessionID() != "") {
        const element = document.querySelector("#mod-btn-" + id);
        if (element.classList.contains("COMMOD")) {
            element.classList.remove("COMMOD");
        } else {
            element.classList.add("COMMOD");
        }
    } else {
        window.location.href = "/Login";
    }
}

function toggleFollowVisual(id) {
    if (getSessionID() != null && getSessionID() != "") {
        const element = document.querySelector("#followButton");
        if (element.classList.contains("SUBSCRIBED")) {
            element.classList.remove("SUBSCRIBED");
        } else {
            element.classList.add("SUBSCRIBED");
        }
    } else {
        window.location.href = "/Login";
    }
}

function menuClose() {
    document.getElementById("overlay-toggle").checked = false;
    var siteOverlay = document.getElementById('site-overlay');
    siteOverlay.classList.remove("active-" + $active_menu);

}

// toggle colour add or remove the class depending on what class is already on the body
function toggleMode() {
    const element = document.querySelector("body");
    if (element.classList.contains("darkMode")) {
        element.classList.remove("darkMode");
        element.classList.add("lightMode");
    } else if (element.classList.contains("lightMode")) {
        element.classList.remove("lightMode");
        element.classList.add("darkMode");
    } else {
        element.classList.add("lightMode");
    }

}

function postAdminToggle(id) {
    const element = document.querySelector(id);
    if (element.classList.contains("hide")) {
        element.classList.remove("hide");
    } else {
        element.classList.add("hide");
    }
}

function toggleComments(id) {
    const element = document.querySelector(id);
    if (element.classList.contains("active")) {
        element.classList.remove("active");
    } else {
        element.classList.add("active");
    }
}

function setPostCommunity(communityName) {
    community = communityName;
}

function setPostFlair(flairName) {
    flair = flairName;
}

function messageUser(user) {
    if (getSessionID() != null && getSessionID() != "") {
        menuActive(2);
        loadMessages(user);
    } else {
        redirectLogin();
    }
}

function deletePost(id) {

}


function toggleEditSetting(n) {
    const textArea = document.querySelector("#settings-text-" + n);
    const inputArea = document.querySelector("#settings-input-" + n);

    if (textArea.classList.contains("display-n")) {
        textArea.classList.remove("display-n");
        inputArea.classList.add("display-n");
        var editValue = inputArea.value;
        textArea.innerHTML = editValue;


    } else {
        if (inputArea.classList.contains("display-n")) {
            inputArea.classList.remove("display-n");
            textArea.classList.add("display-n");
            var editValue = textArea.innerHTML;
            inputArea.value = editValue;
            document.getElementById("settings-input-" + n).focus();
        }
    }
}

function Gamesfocus() {
    document.querySelector(".bootstrap-tagsinput input").focus();
}

// check if on settings page in which case change the body background
if (document.querySelector('.arc-com-settings-page') !== null) {
    var body = document.body;
    body.classList.add("body-settings-page");

}


var allInputs = document.querySelectorAll('input[class^=settings-input]');


// get all settings inputs and watch for keyup event
for (var i = 0; i < allInputs.length; i++) {
    allInputs[i].addEventListener("keyup", function (event) {
        if (event.keyCode === 13) {
            // stop default
            event.preventDefault();

            // get specific id from element ID to pass to the toggle on enter
            var j = this.id.lastIndexOf('-')
            var n = this.id.substring(j + 1);
            toggleEditSetting(n);
        }
    });
}

function navigateMyProfile() {
    if (getSessionID() != null && getSessionID() != "") {
        window.location.href = "/Profile";
    } else {
        window.location.href = "/Login";
    }
}

setTimeout(searchChat, 1000);
function searchChat() {
    var chatsearch = document.getElementById('userSearch').value;
    if (chatrecent == null || chatrecent != chatsearch) {
        getUsers();
        setTimeout(searchChat, 500);
        chatrecent = chatsearch;
    }
}

