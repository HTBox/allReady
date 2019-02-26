/// <binding AfterBuild='default' Clean='clean' ProjectOpened='default' />
"use strict";

const gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify-es").default,
    ts = require("gulp-typescript"),
    fs = require("fs");

const tsProject = ts.createProject("tsconfig.json");

const paths = {
    webroot: "./wwwroot/"
};

paths.ts = paths.webroot + "{ts,js}/**/*.ts";
paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", done => rimraf(paths.concatJsDest, done));
gulp.task("clean:css", done => rimraf(paths.concatCssDest, done));
gulp.task("clean", gulp.series(["clean:js", "clean:css"]));

gulp.task("build:ts", done => {
    tsProject.src()
        .pipe(tsProject())
        .js.pipe(gulp.dest(paths.webroot));
    done();
});

gulp.task("build:lib", done => {
    // need some local functions
    function getNPMDependencies() {
        var buffer, packages, keys;
        buffer = fs.readFileSync("package.json");
        packages = JSON.parse(buffer.toString());
        keys = [];
        for (var key in packages.dependencies) {
            keys.push(key);
        }
        return keys;
    }
    function copyNodeModules(name) {
        gulp.src("node_modules/" + name + "/**/*")
            .pipe(gulp.dest("wwwroot/lib/" + name + "/"));
    }

    // do the real work
    var dependencies = getNPMDependencies();
    for (var i in dependencies) {
        copyNodeModules(dependencies[i]);
    }
    done();
});

gulp.task("min:js", () => {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", () => {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", gulp.series(["min:js", "min:css"]));

// A 'default' task is required by Gulp v4
gulp.task("default", gulp.series(["build:lib", "build:ts","min"]));
