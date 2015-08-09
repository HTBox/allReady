(function ($, ko) {
    var skills = [
        new Skill()
    ];
    var activityAdminSkillsViewModel = function (skills) {
        this.skills = ko.observableArray(skills)
        this.addSkill = function () {
            this.skills.push(new Skill());
        }.bind(this);
        this.removeSkill = function (skill) {
            this.skills.remove(skill)
        }.bind(this);
    };

    function Skill(skill) {
        skill = skill || {};
        this.Name = ko.observable(skill.Name);
        this.Description = ko.observable(skill.Description);
        this.StartDate = ko.observable(skill.StartDate);
        this.EndDate = ko.observable(skill.EndDate);
        this.LocationNeeded = ko.observable(skill.LocationNeeded);
        this.NumberRequired = ko.observable(skill.NumberRequired);
    }

    ko.applyBindings(new activityAdminSkillsViewModel(skills));
})($, ko);