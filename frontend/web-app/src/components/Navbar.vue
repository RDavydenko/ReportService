<template>
  <nav class="blue">
    <div class="nav-wrapper">
      <router-link to="/" class="brand-logo">Home</router-link>
      <ul id="nav-mobile" class="right hide-on-med-and-down">
        <li><router-link to="/">Home</router-link></li>
        <li><router-link to="/users">Users</router-link></li>
        <li><router-link to="/reports">Reports</router-link></li>
      </ul>
    </div>
  </nav>
</template>

<script>
export default {

}
</script>

<style scoped>
  .nav-wrapper {
    padding: 0 20px;
  }
</style>