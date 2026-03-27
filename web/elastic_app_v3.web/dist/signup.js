// src/application/identity/signup.ts
var form = document.getElementById("signup-form");
var messageEl = document.getElementById("message");
form.addEventListener("submit", async (event) => {
  event.preventDefault();
  const request = {
    firstName: document.getElementById("firstName").value,
    lastName: document.getElementById("lastName").value,
    userName: document.getElementById("userName").value,
    password: document.getElementById("password").value,
    reEnteredPassword: document.getElementById("reEnteredPassword").value
  };
  try {
    const response = await fetch("http://localhost:8081/elastic-app/v1/user/signup", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(request)
    });
    if (!response.ok) {
      let problem = null;
      try {
        const json = await response.json();
        if (typeof json === "object" && "title" in json && "detail" in json && "status" in json && "type" in json) {
          problem = json;
        }
      } catch {
        problem = null;
      }
      if (problem) {
        messageEl.textContent = `${problem.title}: ${problem.detail}`;
      } else {
        messageEl.textContent = `Unexpected error occurred: 
 Status: ${response.status}.`;
      }
      messageEl.style.color = "red";
      return;
    }
    form.reset();
    window.location.href = new URL("login.html", window.location.origin).toString();
  } catch (err) {
    messageEl.textContent = "Network error. Please try again.";
    messageEl.style.color = "red";
    console.error(err);
  }
});
//# sourceMappingURL=signup.js.map
