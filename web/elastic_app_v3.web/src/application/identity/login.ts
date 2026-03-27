import type { LoginRequest } from "../../dtos/LoginRequest";
import type { ProblemDetails } from "../../errors/ProblemDetails";

const form = document.getElementById("login-form") as HTMLFormElement;
const messageEl = document.getElementById("message") as HTMLParagraphElement;

form.addEventListener("submit", async (event) => {
    event.preventDefault();

    const request: LoginRequest = {
        userName: (document.getElementById("userName") as HTMLInputElement).value,
        password: (document.getElementById("password") as HTMLInputElement).value,
    };

    try {
        const response = await fetch("http://localhost:8081/elastic-app/v1/user/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(request)
        });

        if (!response.ok) {
            let problem: ProblemDetails | null = null;
            //should reset form if request fails
            try {
                const json = await response.json();
                if (
                    typeof json === "object" &&
                    "title" in json &&
                    "detail" in json &&
                    "status" in json &&
                    "type" in json //this sucks, see if there is a cleaner way of verifying we have the expected ProblemDetails structure returned to client
                ) {
                    problem = json as ProblemDetails;
                }
            } catch {
                problem = null;
            }

            if (problem) {
                messageEl.textContent = `${problem.title}: ${problem.detail}`;
            } else {
                messageEl.textContent = `Unexpected error occurred: \n Status: ${response.status}.`;
            }
            messageEl.style.color = "red";
            return;
        }

        form.reset(); 

        window.location.href = new URL("home.html", window.location.origin).toString();

    } catch (err) {
        messageEl.textContent = "Network error. Please try again."; 
        messageEl.style.color = "red";
        console.error(err);
    }
});