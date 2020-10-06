<template>
  <div>
    <ul>
      <li>
        <router-link tag="a" to="/users">Пользователи</router-link>
      </li>
      <li>
        <router-link tag="a" to="/reports">Отчеты</router-link>
      </li>
    </ul>
  </div>
</template>

<script>
export default {};
</script>

<style scoped>
	li {
		font-size: 30px;
	}
	a {
		text-decoration: underline;
	}
</style>