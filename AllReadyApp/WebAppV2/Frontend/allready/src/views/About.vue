<template>
    <div>
        <p>
        <router-link to="/about">About AllReady</router-link> /
        <router-link to="about/aesop">Aesop's Tale</router-link>
        </p>

        <router-view></router-view>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import AboutAllready from '@/components/AboutAllready.vue';

export default Vue.extend({
  name: 'about',
  components: {
    AboutAllready,
  },
});
</script>
