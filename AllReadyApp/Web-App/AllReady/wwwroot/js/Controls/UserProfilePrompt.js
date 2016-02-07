var HTBox;
(function (HTBox) {
    var UserProfilePrompt = (function () {
        function UserProfilePrompt(container, showUserProfileMessage) {
            var _this = this;
            var _showUserProfileMessage;
            var localStorageKey = "ShowUserProfileMessage";
            if (showUserProfileMessage === null) {
                if (localStorage.getItem(localStorageKey) === undefined)
                {
                    _showUserProfileMessage = false;
                } else
                {
                    _showUserProfileMessage = JSON.parse(localStorage.getItem(localStorageKey));
                }
            } else {
                _showUserProfileMessage = showUserProfileMessage;
                localStorage.setItem(localStorageKey, JSON.stringify(_showUserProfileMessage));
            }

            if (_showUserProfileMessage) {
                container.addClass("warning");
            }
        }
        return UserProfilePrompt;
    })();
    HTBox.UserProfilePrompt = UserProfilePrompt;
})(HTBox || (HTBox = {}));
