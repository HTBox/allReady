angular.module('allReady.controllers', ['Backend']);
angular.module("Backend", ['Backend']); //TODO get a better name for factory and module

angular.module('allReady', ['ionic', 'allReady.controllers', 'Backend'])

  .run(function ($ionicPlatform) {
    $ionicPlatform.ready(function () {
      // Hide the accessory bar by default (remove this to show the accessory bar above the keyboard
      // for form inputs)
      if (window.cordova && window.cordova.plugins.Keyboard) {
        cordova.plugins.Keyboard.hideKeyboardAccessoryBar(true);
        cordova.plugins.Keyboard.disableScroll(true);

      }
      if (window.StatusBar) {
        // org.apache.cordova.statusbar required
        StatusBar.styleDefault();
      }
    });
  })

  .config(function ($stateProvider, $urlRouterProvider) {

    $stateProvider
      .state('app', {
        url: '/app',
        abstract: true,
        templateUrl: 'menu/menu.html',
        controller: 'MenuController'
      })
      .state("app.eventdetails", {
        url: "/eventdetails/:id",
        views: {
          'menuContent': {
            templateUrl: "event/event-details.tpl.html",
            controller: "EventDetailsController",
            params: {
              id: null  
            },
            resolve: {
              eventDetails: function (Backend, $stateParams) {
                return Backend.getEvent($stateParams.id);
              }
            }
          }
        }
      })
      .state("app.checkin", {
        url: "/checkin",
        views: {
          'menuContent': {
            templateUrl: "checkin/checkin.tpl.html",
            controller: "CheckinController"
          }
        }

      })
      .state("app.eventlist", {
        url: "/eventlist",
        views: {
          'menuContent': {
            templateUrl: "event/event-list.tpl.html",
            controller: "EventListController"
          }
        }
      })
      .state("app.login", {
        url: "/login",
        views: {
          'menuContent': {
            templateUrl: "login/login.html"
          }
        }
      });

    $urlRouterProvider.otherwise("/app/eventlist");
    // Other individual routes are defined in respective controllers


  });
