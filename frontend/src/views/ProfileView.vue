<template>
    <div class="min-h-screen bg-gray-100">
        <header class="bg-blue-600 text-white p-5 shadow-lg">
            <h1 class="text-3xl font-bold text-center">User Dashboard</h1>
        </header>

        <main class="container mx-auto p-5">
            <section class="mb-6">
                <h2 class="text-2xl font-bold mb-3">Your Balance</h2>
                <div class="bg-white shadow-md p-6 rounded-lg text-center">
                    <p class="text-4xl font-bold text-green-600">
                        ${{ formatBalance(user.Balance) }}
                    </p>
                </div>
            </section>

            <section class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <div class="bg-white shadow-lg rounded-lg p-5">
                    <h3 class="text-xl font-bold mb-3">Your Bets</h3>
                    <table class="table-auto w-full text-left">
                        <thead>
                            <tr>
                                <th class="p-2 bg-gray-800 text-white">Match ID</th>
                                <th class="p-2 bg-gray-800 text-white">Prediction</th>
                                <th class="p-2 bg-gray-800 text-white">Amount</th>
                                <th class="p-2 bg-gray-800 text-white">Payout</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="bet in bets" :key="bet.MatchId" class="border-b">
                                <td class="p-2">{{ bet.MatchId }}</td>
                                <td class="p-2 capitalize">{{ bet.Prediction }}</td>
                                <td class="p-2">${{ formatBalance(bet.Amount) }}</td>
                                <td class="p-2">
                                    <span v-if="bet.IsResolved">
                                        ${{ formatBalance(bet.Payout) }}
                                    </span>
                                    <span v-else class="text-yellow-500">Pending</span>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <div class="bg-white shadow-lg rounded-lg p-5">
                    <h3 class="text-xl font-bold mb-3">Upcoming Matches</h3>
                    <ul>
                        <li v-for="match in upcomingMatches" :key="match.Id" class="border-b p-2">
                            {{ match.Team1 }} vs {{ match.Team2 }} - {{ match.Date }}
                        </li>
                    </ul>
                </div>

                <div class="bg-white shadow-lg rounded-lg p-5">
                    <h3 class="text-xl font-bold mb-3">Finished Matches</h3>
                    <ul>
                        <li v-for="match in finishedMatches" :key="match.Id" class="border-b p-2">
                            {{ match.Team1 }} ({{ match.Team1Score }}) - {{ match.Team2 }} ({{ match.Team2Score }})
                        </li>
                    </ul>
                </div>
            </section>
        </main>
    </div>
</template>

<script>
import axios from "axios";

export default {
    data() {
        return {
            user: {},
            bets: [],
            upcomingMatches: [],
            finishedMatches: [],
        };
    },
    methods: {
        async fetchUserDashboard() {
            try {
                const token = "JOUW_BEARER_TOKEN"; 
                const headers = { Authorization: `Bearer ${token}` };

                const userRes = await axios.get("/api/user/me", { headers });
                this.user = userRes.data;

                const betsRes = await axios.get("/api/bet/user-bets", { headers });
                this.bets = betsRes.data;

                const matchesRes = await axios.get("/api/matches");
                this.upcomingMatches = matchesRes.data.filter((m) => !m.IsFinished);
                this.finishedMatches = matchesRes.data.filter((m) => m.IsFinished);
            } catch (error) {
                console.error("Error loading dashboard data:", error);
            }
        },
        formatBalance(value) {
            return value ? parseFloat(value).toFixed(2) : "0.00";
        },
    },
    created() {
        this.fetchUserDashboard();
    },
};
</script>

<style>
.table-auto {
    width: 100%;
    border-collapse: collapse;
}

th,
td {
    padding: 8px;
    text-align: center;
}

thead th {
    background-color: #1f2937;
    color: white;
}

tbody tr:nth-child(even) {
    background-color: #f1f5f9;
}
</style>