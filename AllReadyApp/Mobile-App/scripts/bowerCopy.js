/*
 * This hook copies into www/lib the actual contents of packages
 * installed by Bower, since Bower installs some packages' entire
 * source repositories.
 *
 * This results in about a 2x reduction in the size of www/lib.
 */

/* eslint-env node */

"use strict";

var fs   = require("fs");
var fse  = require("fs-extra");
var path = require("path");

// constants
var BOWER_DIR           = "bower_components"; // created by bower
var LIB_DIR             = "www/lib"; // created by this script
var LIB_DIR_PERMISSIONS = parseInt("774", 8); // rwxrwxr-x

var NONSTANDARD_DIST_PATHS = {
    "moment":            path.join("moment", "min"),
    "ionic":             path.join("ionic", "release"),
    "angular-ui-router": path.join("angular-ui-router", "release")
};

module.exports = function(context) {

    // create the lib directory
    fse.mkdirsSync(LIB_DIR, LIB_DIR_PERMISSIONS);

    // copy over the packages
    fs.readdirSync(BOWER_DIR).forEach(function (packageName) {

        var nonStandardPath = NONSTANDARD_DIST_PATHS[packageName];
        var distPath;

        // if there is no non-standard path given for the
        // package, then copy the directory with its name
        // (i.e. copy all of its contents)
        if (typeof nonStandardPath === "undefined") {
            distPath = packageName;
        } else {
            distPath = nonStandardPath;
        }

        var srcPath  = path.join(BOWER_DIR, distPath);
        var destPath = path.join(LIB_DIR, packageName);

        console.log(srcPath + " -> " + destPath);
        fse.copySync(srcPath, destPath);
    });
};
