#!/usr/bin/env node

module.exports = function(grunt) {

  // Project configuration.
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    uglify: {
      options: {
        banner: '/*! <%= pkg.name %> version: <%= pkg.version %>\n*  <%= grunt.template.today("yyyy-mm-dd") %>\n*  Author: Bill Pullen\n*  Website: http://billpull.github.com/knockout-bootstrap\n*  MIT License http://www.opensource.org/licenses/mit-license.php\n*/\n'
      },
      build: {
        src: 'src/<%= pkg.name %>.js',
        dest: 'build/<%= pkg.name %>.min.js'
      }
    },
    jshint: {
      options: {
        reporter: "checkstyle",
        reporterOutput: "jshint.xml"
      },
      all: ['src/knockout-bootstrap.js'],
    }
  });

  // Load the plugin that provides the "uglify" task.
  grunt.loadNpmTasks('grunt-contrib-uglify');
  grunt.loadNpmTasks('grunt-contrib-jshint');

  // Default task(s).
  grunt.registerTask('default', ['uglify', 'jshint']);
  grunt.registerTask('tests', ['default', 'jshint']);

};