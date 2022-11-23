import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
  stages: [
    { duration: "20s", target: 100 }, // simulate ramp-up of traffic from 1 to 100 users over 20 sec minute.
    { duration: "30s", target: 100 }, // stay at 100 users for 30 sec
    { duration: "10s", target: 0 }, // ramp-down to 0 users
  ],
  thresholds: {
    http_req_duration: ["p(99)<2000"], // 99% of requests must complete below 2s
  },
};

export default () => {
  const url =
    "https://localhost:5001/V1.0/todos";

  // const payload = JSON.stringify({
  // });

  const params = {
    headers: {
      "Content-Type": "application/json",
      "Authorization": "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwic3ViIjoiYWRtaW4iLCJqdGkiOiJiNmRkNjY3NyIsInJvbGUiOiJhZG1pbiIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjQ3NzQzIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNzEiLCJodHRwOi8vbG9jYWxob3N0OjUwMDAiLCJodHRwczovL2xvY2FsaG9zdDo1MDAxIl0sIm5iZiI6MTY2OTAyODgwNywiZXhwIjoxNjc2OTc3NjA3LCJpYXQiOjE2NjkwMjg4MDgsImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.IqbUBmGV3ZFJoFHme17Ap4C2hMYlZkGIPO1JiyCnZOc"
    },
  };

  // var res = http.put(url, payload, params);
  var res = http.get(url, params);
  
  sleep(1);
};