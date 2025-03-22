# Pachinko

*Hugo A. Akitaya, Erik D. Demaine, Martin L. Demaine, Adam Hesterberg, Ferran Hurtado, Jason S. Ku, Jayson Lynch*

## Abstract

Inspired by the Japanese game Pachinko, we study simple (perfectly "inelastic" collisions) dynamics of a unit ball falling amidst point obstacles (pins) in the plane. A classic example is that a checkerboard grid of pins produces the binomial distribution, but what probability distributions result from different pin placements? In the 50–50 model, where the pins form a subset of this grid, not all probability distributions are possible, but surprisingly the uniform distribution is possible for {1, 2, 4, 8, 16} possible drop locations. Furthermore, every probability distribution can be approximated arbitrarily closely, and every dyadic probability distribution can be divided by a suitable power of 2 and then constructed exactly (along with extra "junk" outputs). In a more general model, if a ball hits a pin off center, it falls left or right accordingly. Then we prove a universality result: any distribution of n dyadic probabilities, each specified by k bits, can be constructed using O(nk²) pins, which is close to the information-theoretic lower bound of Ω(nk).

## 1. Introduction

Pachinko is a popular mechanical gambling game found in tens of thousands of arcade parlors throughout Japan. The player fires Pachinko balls (ball bearings) into a vertical, nearly two-dimensional area filled with an array of horizontal pins, spinners, winning pockets which reward the player with more balls, etc. See Figs. 1 and 2. Since their invention in the 1920s until the 1980s, balls launched using a mechanical flipper, similar to pinball machines, while more recent Pachinko machines feature electrically controlled automatic ball launching and slot-machine elements. The name "Pachinko" (パチンコ) comes from the Japanese word "pachi pachi" (パチパチ) which imitates the sound of (in this case) metal balls hitting metal pins.

In this paper, we study an idealized geometry and dynamics of a simple form of Pachinko: a single ball in a vertical plane falling through an arrangement of pins, modeled as points.

Our study is motivated by a mathematical illustration/toy built by Jin Akiyama, exhibited at KyotoCGGT 2007 and written about in 2008; see Fig. 3. This device features a regular, checkerboard grid of pins, with a reservoir of wooden balls above, and a bank of containers below which allow a visual measurement of how many balls go where. If we model a ball hitting a pin as having a 50–50 chance of going left or right, then the balls produce a binomial distribution, which we visually recognize as a bell curve. A similar illustration is in Eameses' famous Mathematica exhibit at the Boston Museum of Science (since 1961).

Akiyama's device can be augmented by wooden slats which force balls to go to a particular direction (left or right) when hitting certain pins, dramatically affecting the resulting distribution of balls (as in Fig. 3). This type of forced ramp is common in real Pachinko machines as well, simulated by an array of closely spaced pins (see Figs. 1 and 2).

The central problem addressed in this paper is what distributions of balls can result purely from pins. We consider three models/restrictions on the pins. In the 50–50 model, a ball is only allowed to hit a pin dead on, and thus fall to the left or right of the pin with equal probability. In the general model, pins can be closely spaced, effectively allowing pins to force the direction of the ball. In both cases, we imagine the ball only falling, not bouncing off of pins—effectively modeling perfectly "inelastic" collisions—and measure the probability of a ball reaching each column on the bottom (outputs). Because the only random element in our Pachinko models is a 50–50 pin, all output probabilities are dyadic, i.e., of the form i/2ᵏ for integers i and k. Such probabilities can be represented finitely in binary, e.g., 0.10010111.

While this work is motivated by modeling Pachinko, the theory developed has real-world application in the design of liquid distribution systems. Given an input stream of liquid, the goal is to develop a network of pipes to output a specified amount of liquid into many different containers, where each pipe junction equally distributes liquid between two outgoing children, something easy to manufacture in practice. The specific case of generating uniform distributions is a passive solution to distributing equal amounts of liquid into bottles, and we provide uniform distribution networks for some practical numbers of outputs.

### Our results

In the general model, we prove a universality result: any distribution of n dyadic probabilities (summing to 1) can be produced by a polynomial-size arrangement of pins. If all the probabilities can be specified in binary by k bits (i.e., the probabilities can be written with common denominator 2ᵏ), then we show a constructive upper bound of O(nk²) pins and an information-theoretic lower bound of Ω(nk). These results leave a gap of Θ(k) as an intriguing open problem.

The 50–50 model proves much more interesting because not all probability distributions are possible. For example, the only possible two-probability distribution is ⟨½, 0, ½⟩. Nonetheless, we prove several strong positive results:

1. For every probability distribution ⟨p₁, p₂, ..., pₙ⟩, and for every ε > 0, there is a 50–50 construction producing an output probability within ε of each pᵢ, as well as extra outputs with probability less than ε.

2. For every dyadic probability distribution ⟨p₁, p₂, ..., pₙ⟩, there is a constant scale factor α = 1/2ʲ and a 50–50 construction producing output probability αpᵢ (i.e., a shifted version of pᵢ) for each i, as well as additional garbage outputs.

3. While the uniform distribution is intuitively very difficult to produce in the 50–50 model, we show that it is in fact possible for n ∈ {1, 2, 4, 8, 16} outputs, and conjecture that it is possible for all n = 2ᵏ.

Because not all dyadic probability distributions are possible in the 50–50 model, the first two results are in some sense best possible. Nonetheless, a natural open question is to characterize exactly which probability distributions are possible in the 50–50 model. In particular, is every dyadic probability constructible, ignoring all other output probabilities? We conjecture so, but currently only know (by the first result) that every probability can be approximated arbitrarily closely.

## 2. Models

In this section, we define a simple formal model for Pachinko dynamics, and then specialize to the 50–50 model and another simple grid model. The environment is a planar arrangement of fixed point obstacles called pins; and a disk called a ball which starts at coordinate (x, ∞) for an input horizontal value x. Without loss of generality, we will only consider Pachinkos with balls input at coordinate x = 0 with unit diameter, since the dynamics of a Pachinko with a non-unit diameter input ball centered somewhere else can be equivalently simulated via a scaling and translation. A Pachinko is then the set of pins in the environment together with the ball's input position. Given a pin p, we will notate its horizontal location as pₓ and its height as p_y.

The ball falls continuously straight down until obstructed by a pin; see the left of Fig. 4. If the bottom half of the ball touches a pin whose x coordinate is (left, right) of the ball center, then the ball rotates (clockwise, counterclockwise) around the pin until the pin reaches the (leftmost, rightmost) point of the ball, no longer obstructing the ball's downward trajectory. If the x coordinates match, then the flip of a fair coin determines the roll direction.

In any case, the ball's rolling around a pin may at some point be obstructed by another pin or pins. If all obstructing pins are either directly below or to one side of the ball center, the ball rotates away from the obstructing pins around the obstructing pin horizontally closest to the ball's center. Alternatively, if obstructing pins exist on both sides of the ball center, then the ball stops. We call the ball stuck in this case, coming to rest at a rest site sᵢ, the location of the bottom of the ball at rest. This condition is illustrated in the center of Fig. 4. The other possibility for termination is that the ball reaches (x'ᵢ, −∞) for some output value x'ᵢ. We call the ball dropped in this case, with (x'ᵢ, −∞) a drop site. We call rest and drop sites collectively as outputs to the Pachinko. The number of outputs is trivially bounded from above by twice the number of pins.

A 50–50 Pachinko is a Pachinko with the property that whenever a ball hits a pin, it hits dead on so that the ball rotates around the pin to either side with equal probability. This model is equivalent to requiring that the pins lie horizontally at integer values away from zero, the initial x coordinate of the ball, such that the ball can never hit two pins at the same time. But since decreasing the vertical distance between pins of such a Pachinko while maintaining that the ball only hits one pin at a time will not change the output dynamics, without loss of generality we can compress pins to lie on a unit equilateral triangular grid, see the right of Fig. 4. This observation motivates the study of Grid Pachinkos in Section 2.2.

### 2.1 Pachinko graph

It would be convenient to abstract away the geometry of a Pachinko and represent the Pachinko as an "equivalent" graph that is easier to analyze.

Consider the augmented directed graph with vertices corresponding to the pins, the ball input, and the outputs of a Pachinko. We store with each vertex with a location: the location of the pin for pins, (0, ∞) for the ball input, (x', −∞) for drop sites, and the location of the bottom of a stuck ball for rest sites. As mentioned previously, the number of outputs is at most linear in the number of pins, so the number of vertices in a Pachinko graph is also at most linear. We add arcs to the directed graph in the following three ways.

(a) We add an arc from pin or input p to pin or drop site q if an input ball can hit or drop to q directly after and not at the same time as hitting p, without getting stuck or also hitting another pin.

(b) If a ball hitting q directly after hitting p gets stuck, add an arc from both p and q to a vertex corresponding to the rest site s.

(c) Lastly, if a ball hits more than one vertex directly after hitting p without getting stuck, add arcs in a chain from p to the highest incident vertex, and from the highest incident vertex to the next highest and so on.

Fig. 5 illustrates these cases. We call the resulting augmented directed graph a Pachinko graph. For a Pachinko containing n pins, we will show how to construct this graph in O(n log n) time, but first we will analyze some of the properties of Pachinko graphs.

**Lemma 1.** For every arc in a Pachinko graph, the pin corresponding to the tail is strictly higher than the pin or output corresponding to the head unless they are at the same location. The horizontal distance traversed by any arc must be less than one ball width.

**Proof.** To prove the first claim, assume for contradiction there exists arc (p, q) with p_y ≤ q_y with p ≠ q. If q is not a rest site, an input ball hitting q directly after hitting p would rotate around the top of p in the direction of q and then necessarily get stuck between them, a contradiction. Alternatively, q is a rest site with p on the circle with bottom most point at q. But since p ≠ q, that means p_y > q_y a contradiction.

To prove the second claim, after rotating around any pin a ball drops directly down with the pin on its side. The ball cannot move horizontally without interacting with a pin, so a ball cannot hit a pin more than a ball width away from the starting pin without hitting another pin first. □

**Lemma 2.** The straight-line embedding of a Pachinko graph is acyclic and planar, with out-degree at most two at any vertex.

**Proof.** To prove acyclicity, assume for contradiction that a directed cycle exists. The cycle cannot contain outputs as vertices corresponding to drop and rest sites have no children. By Lemma 1, the parent of every vertex then corresponds to a higher pin, so there can be no highest one, a contradiction.

To prove planarity, assume for contradiction that two arcs (a, b) and (c, d) cross with a_y > b_y and c_y > d_y; see Fig. 6. Without loss of generality, assume a_y ≥ c_y > b_y, and |a_x − c_x| ≥ 1 or else a ball from a would hit c before hitting or outputting at b. By Lemma 1, |a_x − b_x| < 1 and |c_x − d_x| < 1, so both b and d are horizontally between a and c. Since the arcs cross, d must be horizontally between a and b, and b must be horizontally between c and d. Then b cannot be above d or else a ball from c would hit or output at b before d, and d cannot be above b or else a ball from a would hit or output d before b, a contradiction.

To prove every vertex has max out-degree at most two, first note that every output has out-degree zero. Further, arcs can leave pin location p only when a ball hitting p hits another pin or outputs directly after rolling to the left or the right. Suppose for contradiction that more than two arcs leave p, so two arcs to pins/outputs q and r exist to one side of p. So q and r are reached directly after rolling around p to one side, and are reached at the same time. These arcs cannot be constructed by construction method: (a) because q and r are reached at the same time; (b) because arcs would only be added to the rest site; nor (c) because an arc would only be added to the higher of q or r. So arcs to q and r cannot exist, a contradiction. □

Since a Pachinko graph with n pins has a linear number of vertices and bounded out-degree, it follows directly that Pachinko graphs have a linear number of edges; so storing a Pachinko graph along with vertex locations requires Θ(n) space.

**Theorem 3.** A Pachinko graph can be constructed from a Pachinko with n pins in O(n log n) time.

To prove this Theorem, we will construct the Pachinko graph by inductively constructing increasing subsets of the Pachinko graph, each containing only the arcs terminating at one of the k highest pins. At each step, we maintain a Pachinko graph subset, and a sorted list of action sites ordered by horizontal position.

First we sort the pins by height to schedule which pin to add next, breaking ties left before right. Next, we construct the Voronoi diagram of the Pachinko pins, which has linear size. Each Voronoi cell Vᵢ contains one pin pᵢ. Consider the highest point hᵢ in Vᵢ also contained in the unit diameter disk centered at pᵢ. If hᵢ is not on the unit diameter semicircle above and centered at pᵢ, then pᵢ can never be reached by the input ball since pins in adjacent Voronoi cells would block any ball from touching pᵢ. Alternatively if hᵢ is on the semicircle, for each pin store the left and right endpoints ℓᵢ and rᵢ of the largest arc of the semicircle completely inside Vᵢ containing h.

**Lemma 4.** Any ball hitting pᵢ will have its center on the largest semicircle arc in Vᵢ containing hᵢ.

**Proof.** Suppose for contradiction a ball hitting pᵢ does not have its center on this arc, and instead hits some point h' on some other disconnected arc of the semicircle. Then at least one Voronoi edge to some higher pin pⱼ lies between the two disconnected arcs, with pᵢ between pⱼ and hᵢ since hᵢ is at least as high as h'ᵢ. But then pⱼ is above h'ᵢ blocking any ball from reaching h'ᵢ, a contradiction. □

A ball hitting pᵢ may proceed to roll around it until it reaches ℓᵢ or rᵢ, at which point it will stop or leave contact with pᵢ. We call the input site together with the set of ℓᵢ and rᵢ for each pin action sites. The action sites will serve as infrastructure to construct the arcs between pins, the input site, and the output sites (drop sites and rest sites) that form the Pachinko graph. When we construct an arc of the Pachinko graph from an action site, what we really mean is to construct an arc from the input site if the action site is the input site, or from pin pᵢ if the action site is ℓᵢ or rᵢ. We say that ℓᵢ and rᵢ correspond to pin pᵢ.

We label each endpoint as either DROP or REST depending on if a ball reaching the endpoint will continue to move or not. Endpoints always lie on the semicircle above and centered on pᵢ by definition. If the endpoint does not also intersect a Voronoi edge, than it is the endpoint of the semi circle. The ball will then fall away from pᵢ, so we label it DROP. Otherwise, the endpoint lies on a Voronoi edge or vertex, and is exactly a half unit distance from some subset of pins Q ≠ pᵢ, all at or below pᵢ or else the pin above would block a ball from ever reaching the endpoint. If (ℓᵢ, rᵢ) is to the (left, right) of pᵢ and all the pins of Q are below or to the (right, left) of (ℓᵢ, rᵢ), then a ball at the endpoint may continue to move, and we label it DROP. Alternatively if (ℓᵢ, rᵢ) is to the (left, right) of pᵢ and not all the pins of Q are below or to the (left, right) of (ℓᵢ, rᵢ), then a ball at the endpoint will stop by definition, so we label it REST. Lastly, if (ℓᵢ, rᵢ) is above or to the (right, left) of pᵢ, then a ball at the endpoint will rotate around pᵢ to the other endpoint, so we label it DROP. These cases are illustrated in Fig. 7.

For the base case, there are no existing pins or action sites to consider, and the Pachinko graph subset contains no arcs as desired. We will maintain a list of active action sites. Since the ball drops from the input location, we initialize the list of active action sites with the ball's input location (x, ∞) as a DROP action site.

For the inductive case, we are given the subset of the Pachinko graph containing only the arcs terminating at one of the k highest pins and a horizontally sorted list of active sites corresponding to all locations of balls that may leave contact with the k highest pins. Let pᵢ be the k + 1 highest pin, breaking ties left before right, and let Vᵢ be its Voronoi cell. Binary search for the set Aᵢ of all DROP action sites horizontally within half a unit of pᵢ. Now update the Pachinko graph for each DROP action site a ∈ Aᵢ by adding to the Pachinko graph subset an arc from a to pᵢ. To add new active action sites, if there is any action site in Aᵢ to the (left,right) or above pᵢ, add (ℓᵢ, rᵢ) to the sorted active site list, while removing all DROP action sites in Aᵢ from the active site list.

**Lemma 5.** The above procedure constructs the Pachinko graph containing only the arcs terminating at one of the k + 1 highest pins and a horizontally sorted list of active sites corresponding to all locations of ball centers that can leave the k + 1 highest pins.

**Proof.** To prove the first claim, we need only check that the added arcs are exactly the arcs terminating at pᵢ. By construction, all arcs end at pᵢ. Now suppose for contradiction that some arc of the Pachinko graph terminating at pᵢ from pin or the input is not added by the above procedure. If the arc starts from a pin pⱼ, then it is one of the k highest pins by Lemma 1, and either ℓⱼ or rⱼ will be labeled DROP or else a ball hitting pⱼ would not leave. Further, either ℓⱼ or rⱼ (or the input location) must be within a horizontal half unit distance from pᵢ by Lemma 1, with no pin obstructing the path between them. But then pᵢ would have a corresponding DROP action site above it, and the above procedure would construct the corresponding arc to pᵢ, a contradiction.

To prove the second claim, all action sites removed by the procedure certainly would not be able to reach any pin below pᵢ. Further, ℓᵢ and rᵢ correspond to the only locations a ball could leave pᵢ, proving the claim. □

After constructing all the arcs of the Pachinko graph terminating at pins, it remains to construct the arcs to the outputs. Add a drop site at (x', −∞) corresponding to the horizontal position of each active DROP action site and an arc from the action site to the drop site. It is possible that multiple REST endpoints coincide. In any case, we construct a single rest site for all REST endpoints in the same location, with location half a unit directly below the location, and construct an arc from each REST action site at the location to the constructed rest site. The correctness of these constructions follow directly from the definitions of drop, rest, and action sites. An example of this algorithm applied to a Pachinko is shown in Fig. 8.

Now to prove Theorem 3.

**Proof.** Construct the Pachinko graph using the above procedure. Sorting and construction of the Voronoi diagram each takes O(n log n) time. The calculating and labeling intersections between circles centered at pins and the Voronoi cell containing them is at most linear since the number of edges in the Voronoi diagram is linear and a circle can cross a line at most twice, and each can be calculated in constant time. The number of action sites is linear since it is bounded above by twice the number of pins, and binary searching for DROP action sites from the horizontally sorted list of active action sites with location near a pin takes O(log n) time per pin. Further, since each DROP action site is immediately removed after being found, the total search and maintenance time is O(n log n), leading to an O(n log n) total construction time. □

**Theorem 6.** The probability that an input ball hits any pin of a Pachinko or outputs at a rest or drop site can be calculated for all pins and outputs in O(n log n) time.

**Proof.** To calculate probabilities, construct the Pachinko graph, and calculate probabilities in the graph breadth first. First we calculate the probability transferred along each Pachinko graph arc. If the edge starts at the input site, the edge carries probability 1. Otherwise, the arc a starts at a pin p (with out-degree at most two, possibly one to the left and one to the right) and points to a vertex v either on the left or right of p, and we calculate the probability transferred along the arc by summing probabilities coming from the parents of p. For each arc a' terminating at p starting from a vertex u either more than half a unit distance horizontally from p on the same side as v or less than half a unit distance horizontally from p on the opposite side as v, transfer the total probability of a' to a. This assignment is correct because any balls falling from u to p falls with its center on the same side as v, so will always output on that side. Additionally, for each arc a' terminating at p starting from a vertex u exactly half a unit distance horizontally from p, transfer half the probability of a' to a. This assignment is correct because any ball falling from u to p falls centered with p and will split its probability between its two children. These cases are illustrated in Fig. 9. The probability that a ball hits any pin p is then the sum of the probabilities of all arcs into p. All arc probabilities can only contribute to any sum once, so all probabilities can be calculated in breadth first order in linear time. Thus all probabilities can be calculated in O(n) time on top of O(n log n) time needed to construct the Pachinko graph. □

### 2.2 Grid Pachinkos

General Pachinko can have complicated behavior, especially when three or more pins can touch a ball at the same time. Thus in constructing Pachinkos, it will be easier to restrict ourselves to more idealized Pachinkos, namely Pachinkos with pin locations and ball input location restricted to lie on a unit equilateral triangular grid. Without loss of generality, we let the ball input column be column zero, with columns every half unit numbered increasing to the right and decreasing to the left. We will call row 1 the row containing the highest pin at height h, with row k containing pins at height h − k√3/2.

Recall that such a setup results in 50–50 Pachinkos, with each pin causing an incident ball to rotate to the left or right with equal probability. These pins act normally, so we call them N-pins. However, to allow Grid Pachinkos to simulate more of the behavior of a general Pachinko, we allow the placement of L-pins, R-pins, and S-pins at grid vertices, pins where incident balls must roll left, right, or stop respectively. We can simulate Grid Pachinkos using general Pachinko dynamics by simulating N-pins, L-pins, R-pins, and S-pins with pin arrangements contained in δ × δ blocks as shown in Fig. 10, and increasing the distance between each row and column by δ. Since we can simulate Grid Pachinkos with L-pins, R-pins, and S-pins using general Pachinko dynamics, any output distributions we can construct using Grid Pachinkos we can also construct using general Pachinkos with three times the number of pins. A nice property of Grid Pachinkos is that the probability flow through the Pachinko graph is independent of the location of pins, and computable simply from the structure of the Pachinko graph.

## 3. 50–50 Pachinkos

50–50 Pachinkos are a subset of general Pachinkos, and are more restrictive in the possible distributions one can construct.

### 3.1 Invariants on the distribution

Unlike General Pachinkos or Grid Pachinkos that can use L-pins and R-pins to move probability arbitrarily to any location, 50–50 Pachinkos must output all probability to drop sites in a "centered" manner.

**Lemma 7.** If there are k > 1 outputs, they span strictly between k and 2k columns with no two consecutive columns lacking an output.

**Proof.** There must be at least k + 1 columns spanned by the outputs, k for the outputs, and one column necessarily blocked by the lowest hit pin. No two consecutive columns lack an output. Suppose for contradiction two such columns exist for which there exist outputs to the left and right of these columns. Then there must exist hit pins in both columns or else a ball could not travel across them. The lowest such pin in each column cannot be next to each other because of the grid, so one must be above the other. The lower pin will necessarily output in the column of the higher pin, a contradiction. □

**Theorem 8.** If every pin of a 50–50 Pachinko is between columns −t and t, and pᵢ is the probability that the ball outputs in column i, then ∑ᵢ ipᵢ = 0.

**Proof.** The proof is by induction on the number of pins. For 0 pins, it's clearly true. Given an n-pin arrangement, remove a bottommost pin to get an arrangement for which, by induction, ∑ᵢ ipᵢ = 0. Adding a pin at the bottom of column k replaces probability p_k in column k with probability p_k/2 in each of columns k − 1 and k + 1, changing the total by p_k/2(k − 1) − p_k k + p_k/2(k + 1) = 0, as desired. □

However, not every dyadic probability distribution satisfying the above condition is the set of probabilities of a 50–50 Pachinko. For instance, the two output probability distribution ⟨1/4, 0, 3/4⟩ is not constructible by a 50–50 Pachinko as a consequence of Theorem 8, but it is constructible by a General or Grid Pachinko, as shown in the left diagram of Fig. 11. It is still open whether every dyadic probability can be output by a 50–50 Pachinko.

### 3.2 Constructible probabilities

**Open Problem 1.** Is every dyadic rational the output probability of some 50–50 Pachinko?

Table 1 gives all dyadic probabilities that can be output by all possible 50–50 Pachinkos with a small number of rows. This table was constructed by naive exponential exhaustive search. This data suggests that some probabilities require a large number of rows of pins to produce. For example, the probability 3/4 (0.11 in binary) cannot be produced by a 50–50 Pachinko with fewer than nine rows of pins, but it can be produced by a 50–50 Pachinko with eleven rows; see the right diagram in Fig. 11.

### 3.3 Full and truncated Pachinkos

While not all distributions are constructible, in this and the following sections we analyze some interesting 50–50 Pachinkos. In this section, we consider 50–50 Pachinkos containing pins in all possible locations up to row k (first row with pins is row 1, row number increasing down), which we call the Full k-Pachinko. The output probabilities of this Pachinko constitute the (k + 1)th row of Pascal's triangle divided by 2ᵏ; see left of Fig. 12. This fact is readily apparent because the probability at each pin is the sum of the two probabilities above it divided by two.

Another interesting family of 50–50 Pachinkos arises by removing from a Full k-Pachinko all the pins in column m, which we call an m-truncated k-Pachinko; see right of Fig. 12. Without loss of generality, we will assume k > m > 0, the mth column to the right of center. A ball may fall into column m only directly after hitting some pin in column m − 1 in row m + 2j for j ∈ {0, . . . , (k − m)/2}. Let B(m, j) be the number of paths an input ball starting at (0, 1) can take to reach pin (m − 1, m + 2j). Then the probability of a ball falling into column m after hitting pin (m − 1, m + 2j) is then B(m, j)/2^(m+2j). We observe that every path in the pachinko from (0, 1) to (m − 1, m + 2j) must hit either the pin at (−1, 2) or the pin at (1, 2), leading to the recurrence B(m, j) = B(m − 1, j) + B(m + 1, j − 1); see Fig. 13. The numbers that satisfy this recurrence are the Ballot numbers, with:

B(m, j) = (m/(2j + m)) * binomial(2j + m, j).                    (1)

Ballot numbers are a generalization of the Catalan numbers, the special case for m = 1. Ballot numbers have the following generating function:

((1 - √(1 - 4x))/(2x))^m = ∑ⱼ₌₀^∞ B(m, j)x^j.                   (2)

We can use the properties of these numbers to prove two useful Lemmas which we use in Section 4 to construct Pachinkos that can approximate any probability distribution.

**Lemma 9.** The output probability in column m of an m-truncated k-Pachinko approaches 1 as k → ∞.

**Proof.**
∑ⱼ₌₀^∞ B(m, j)/2^(m+2j) = (1/2^m) * ∑ⱼ₌₀^∞ B(m, j)(1/4)^j = (1/2^m) * ((1 - √(1 - 4(1/4)))/(2(1/4)))^m = 1   (3)
□

**Lemma 10.** There exists a finite Pachinko that outputs a ball in column m with probability greater than 1 − ε for any ε > 0.

**Proof.** Consider an m-truncated (m + 2j)-Pachinko for j such that B(m, j + 1)/2^(m+2(j+1)) ≤ ε, which exists because B(m, j) is always positive and lim ⱼ→∞ B(m, j)/4^j = 0 (easily derived from Stirling's approximation for factorials). Then Lemma 9 yields the following lower bound on the output probability in column m:

∑ᵢ₌₀^j B(m, i)/2^(m+2i) = ∑ᵢ₌₀^∞ B(m, i)/2^(m+2i) - ∑ᵢ₌ⱼ₊₁^∞ B(m, i)/2^(m+2i) > 1 - B(m, j + 1)/2^(m+2(j+1)) ≥ 1 - ε.   (4)
□

### 3.4 3 or fewer outputs

For a small number of outputs, constructible output probability distributions of 50–50 Pachinkos can be calculated directly. The only possible outputs for 50–50 Pachinkos with one or two outputs are ⟨1⟩ and ⟨1/2, 0, 1/2⟩ respectively. 50–50 Pachinkos with exactly three outputs can output in either four or five columns by Lemma 7. For four columns, pins only exist in two columns, and hit pins ordered by height must alternate between the two columns as shown on the left of Fig. 14. Such a Pachinko with k hit pins results in the following distributions or their reversals:

⟨∑^(j+1)ᵢ₌₁ 2^(-2i+1), 0, 2^(-k), ∑^j ᵢ₌₁ 2^(-2i)⟩ for k = 2j + 1,    (5)

⟨∑^j ᵢ₌₁ 2^(-2i+1), 2^(-k), 0, ∑^j ᵢ₌₁ 2^(-2i)⟩ for k = 2j.          (6)

50–50 Pachinkos with exactly three outputs in five columns can be represented implicitly by products of linear transformations on a 5-vector encoding the output probability in each column. The ball input is represented by a starting vector chosen from the set

X = {⟨1/2, 0, 1/2, 0, 0⟩ᵀ, ⟨0, 0, 1, 0, 0⟩ᵀ, ⟨0, 0, 1/2, 0, 1/2⟩ᵀ},     (7)

which correspond to a ball starting in column two, three, or four respectively. Then the ball may interact with a sequence of one or more pin arrangements in any of the three patterns shown in the center of Fig. 14.

These patterns correspond respectively to the following three linear transformations:

A = [ 1  1/2  1/4  0    0   ]
    [ 0  0    0    0    0   ]
    [ 0  1/2  1/2  1/2  0   ]
    [ 0  0    0    0    0   ]
    [ 0  0    1/4  1/2  1   ]

B = [ 1  0    0    0    0   ]
    [ 0  1    1/2  0    0   ]
    [ 0  0    1/4  1/2  0   ]
    [ 0  0    0    0    0   ]
    [ 0  0    1/4  1/2  1   ]

C = [ 1  1/2  1/4  0    0   ]
    [ 0  0    0    0    0   ]
    [ 0  1/2  1/4  0    0   ]
    [ 0  0    1/2  1    0   ]
    [ 0  0    0    0    1   ]    (8)

Assuming that the nonzero probability outputs in columns one and five, the sequence of pins may not end with a pin in column three, or with configuration B or C, or else the Pachinko would have four outputs. So the pins must end with an A configuration. Thus, the producible output distributions are exactly the distributions that can be written in the following form, for a sequence of configurations Tᵢ ∈ {A, B, C} and a ball input vector x ∈ X:

A(∏^k ᵢ₌₁ Tᵢ)x.    (9)

An example of one such Pachinko is shown on the right of Fig. 14.

### 3.5 Uniform distributions

Uniform distributions are another special class of outputs that one might like to construct under this model, particularly for liquid distribution applications. Fig. 15 shows 50–50 Pachinkos that output uniform distributions of 1/2, 1/4, 1/8, and 1/16 respectively. Two different 50–50 Pachinkos with a uniform distribution of 1/16 are given, demonstrating that different distributions of pins can yield the same output. It is still open if all uniform distributions of probabilities of the form 1/2ᵏ are constructible by 50–50 Pachinkos.

**Open Problem 2.** Is every uniform distribution of output probabilities of the form 1/2ᵏ constructible by a 50–50 Pachinko?

### 3.6 Any probability distribution, shifted

While one cannot output every dyadic probability distribution by itself using a 50–50 Pachinko, one can output a multiple of any probability distribution, in addition to some other probabilities.

**Theorem 11.** For any ordered set of dyadic probabilities p₁, p₂, ..., pₙ there is a positive constant c and a 50–50 Pachinko for which the probabilities for n of its outputs, in order from left to right, are cp₁, cp₂, ..., cpₙ.

**Proof.** Let m be the maximum of the numerators of the pᵢs. We will construct an example with c = 3/2^(m+2n+10).

For each i ∈ {1, ..., n}, let cpᵢ = 3q'ᵢ/r'ᵢ in lowest terms. For each i ∈ {1, ..., n}, q'ᵢ ≤ m < 2^(m+2i+4), so by doubling numerators and denominators as necessary, let cpᵢ = 3qᵢ/rᵢ where rᵢ is a power of 2 and 2^(m+2i+4) < qᵢ ≤ 2^(m+2i+5). That is, we put the numerators in increasing order and spaced out from each other and from 1.

We create two long diagonals of Pachinko pins as shown on the left of Fig. 16.

The probability that a ball hits the kth pin in the first diagonal is 2^(1-k), and the probability that a ball hits the kth pin in the second diagonal is k2^(-k).

For each i, put three pins in the third diagonal in spots qᵢ − 1, qᵢ, and qᵢ + 1, so the probabilities that balls hit them are (qᵢ − 1)2^(-qᵢ), (2qᵢ − 1)2^(-1-qᵢ), and 3qᵢ2^(-2-qᵢ), respectively. Since q_i−1 < qᵢ − 3 and q_i+1 > qᵢ + 3, they don't interfere.

Finally, the left output of the last of those three pins, P (which is hit with probability 3qᵢ2^(-2-qᵢ)), goes to a spot where no other output goes, so by alternating between pins in that column and P's column, we can get a probability of the form 3qᵢ2^(-2-qᵢ-t) for any t. In particular, since 2 + qᵢ ≤ 2^(m+2i+5) + 2 < 2^(m+2n+10), we can get 3qᵢ2^(-2^(m+2n+10))/rᵢ = cpᵢ, as desired. □

### 3.7 Approximating any probability distribution

While outputting any single dyadic probability with 50–50 Pachinkos remains open, one can approximate any real probability distribution to arbitrary precision.

**Theorem 12.** For any real probability p and any ε > 0, there exists a 50–50 Pachinko with a finite number of pins that outputs a probability in (p − ε, p + ε).

**Proof.** Let m and n be integers such that |m/n − p| ≤ ε/3. Without loss of generality assume n > m > 0.

By Lemma 9, there exists a finite arrangement of pins that achieves probability at least 1 − ε/(3 max(|m|,|m−n|)) in column m, leaving at most ε/(3 max(|m|,|m−n|)) in all other columns combined, and in particular at most ε/(3 max(|m|,|m−n|)) between columns m − n and m, not inclusive. Remove all pins in column m − n from that arrangement. Doing this does not increase the probability that a ball ends between columns m −n and m, exclusive, since it affects only balls that make it to column m − n, and decreases the probability that such a ball ends between columns m − n and m to zero. So the ball ends between m − n and m, exclusive, with probability at most ε/(3 max(|m|,|m−n|)), and otherwise ends in column m − n or m. But the first moment is 0, so if pᵢ is the probability that the ball ends in column i, then ∑ipᵢ = 0, so |mpₘ + (m − n)p_(m−n)| = |∑ᵢ∈(m−n,m) ipᵢ| < max(|m|, |m −n|)ε/(3 max(|m|,|m−n|)) = ε/3. Also, the total probability is 1, so |pₘ + p_(m−n) − 1| ≤ ε/(3 max(|m|,|m−n|)) ≤ ε/3m. By the triangle inequality, |mpₘ + (m − n)p_(m−n)| = |m(pₘ + p_(m−n)) − np_(m−n)| ≥ |m − np_(m−n)| − ε/3, so |m − np_(m−n)| ≤ 2ε/3, so |m/n − p_(m−n)| ≤ 2ε/3, so by the triangle inequality again, |p − p_(m−n)| ≤ ε, as desired. □

**Theorem 13.** For any finite ordered set of real probabilities (p₁, ..., pₙ) and any ε > 0, there exists a 50–50 Pachinko with a finite number of pins for which the probabilities for n of its consecutive outputs, in order from left to right, are in (pᵢ − ε, pᵢ + ε) for i ∈ {1, ..., n}.

**Proof.** Note that the previous proof proves the stronger statement that for any real probability p, any ε > 0, and sufficiently large n, there exists a 50–50 Pachinko with a finite number of pins that takes probability 1 at column i to a probability distribution within statistical distance 2ε of probability p at column i − n and probability 1 − p at some column j ≥ i + n*p/(1−p), without using any pins at columns outside the interval (i − n, j).

We will prove the claim by induction on n, essentially by applying the above in series many times.

The base case n = 1 is the previous theorem. If n ≥ 2, consider the inductive construction of the probability distribution (p₁/(1−pₙ), ..., p_(n−1)/(1−pₙ)) to within ε/3. It uses finitely many pins, so they all lie between columns −n and n for some n. So, first construct a distribution within statistical distance ε/3 of probabilities 1 − pₙ and pₙ separated by at least 2n columns. Then apply that inductive construction to the output with probability 1 − pₙ, giving probabilities p₁, ..., p_(n−1) in consecutive columns, without interfering with the probability of pₙ more than n columns away. Finally, apply Lemma 9 to take the probability pₙ to the column just right of p_(n−1), again to within statistical distance of ε/3, giving (p₁, ..., pₙ) to within statistical distance ε, as desired. (If n = 2, the last step is the only relevant step.) □

## 4. General Pachinkos

### 4.1 Universality

For general Pachinkos, any ordered set of dyadic probabilities may be constructed.

**Theorem 14.** Every finite ordered set of dyadic probabilities are exactly the ordered outputs of some Pachinko.

**Proof.** We can trivially construct a Grid Pachinko that outputs any finite ordered set of probabilities. Construct a complete balanced binary tree with each leaf having probability equal to the lowest bit in the input set. This tree can be constructed by using N-Pins at junctions and L-Pins and R-Pins to move the junctions apart as necessary. Then use L-Pins and R-Pins to combine the leaf output bits together to form the required outputs. Since Grid Pachinkos can be simulated by General Pachinkos, the claim holds. Of course this construction requires the use of an exponential number of pins. □

Thus, the question arises, how many pins does a Pachinko need to output a given dyadic probability distribution? Let (p₁, p₂, ...pₙ) be the target dyadic probability distribution with each pᵢ the probability of the ball terminating in column i. Let 2ᵏ be the largest denominator in any pᵢ, so each probability may be represented using k bits. We show that the constructing an arbitrary dyadic distribution requires at least Ω(nk) pins, and we give an algorithm that constructs the target distribution using O(nk²) pins.

### 4.2 Lower bound on pins

**Theorem 15.** For any n and k there exists an (unordered) set of n k-bit dyadic output probabilities requiring Ω(nk − n log n) Pachinko pins.

Note that since each probability is at least 1/2ᵏ, there are at most 2ᵏ nonzero probabilities, so for reasonable parameter choices the first term dominates and gives a lower bound of Ω(nk) pins.

**Proof.** The number of ordered such probability distributions is ((2ᵏ choose n)^n), since there are that many probability distributions in which the first n − 1 probabilities are chosen independently between 0 and 1/n.

So the number of ordered probability distributions is Ω(2^(nk−n log n)), and the number of unordered probability distributions is also Ω(2^(nk−n log n)), since each unordered probability distribution corresponds to at most n! = O(2^(n log n)) ordered ones, where we have used a weaker relation than the bound n! = O(√(2πn(n/e)^n)) from Stirling's approximation.

A Pachinko arrangement is completely specified by the planar digraph describing which pins' outputs are which pins' inputs. Bonichon et al. showed that the number of (unlabeled) planar graphs on t vertices is O(2^t). A planar digraph consists of a planar graph and an orientation for each of its O(t) edges, so the number of planar digraphs on t vertices is also O(2^t). So at least nk − n log n pins are necessary to even construct enough planar digraphs to generate all the probability distributions. □

### 4.3 Upper bound on pins

In this section, we prove an upper bound of O(nk²) on the same problem as a lower bound was given for in the previous section.

We define a pure stream to be the Pachinko region for which the probability that the center of the ball passes through that region is 1/2^m for any integer m. An impure stream is a region that has ∑^m_i=1(1/2^a_i) of probability that the center of the ball passes through that region with (a₁, ..., a_m) being a strictly increasing integer sequence.

**Lemma 16.** A pure stream 1/2^m and a impure stream ∑^j_i=1(1/2^a_i), with m < a₁ and separated by 3 empty columns, can switch columns using O(a_j − m) pins.

**Proof.** We provide a constructive example in Fig. 17. Without loss of generality, consider that the pure stream is positioned on the left. First, move the pure stream to the center (two columns to the right) using two R-pins. Then, use an N-pin to split the pure stream into two streams of value 1/2^(m+1). Now, we check if 1/2^(m+1) ∈ (a₁, ..., a_j). If that is the case, we add two R-pins: one to direct one of the pure streams to the center and the other to merge it with the impure stream to the right. Otherwise, use two L-pins at the same position. Repeating the splitting process until the pure stream at the center is split into two streams of value 1/2^a_j (a_j − m − 1 times). Now, use an L-pin and an R-pin as shown in Fig. 17.

We call this construction a crossover gadget. With this gadget, we can construct an impure stream of value ∑^j_i=1(1/2^a_i) in the left column, and a pure stream of value 1/2^m in the right column. The construction uses 2 + 3(a_j − m) pins. □

**Lemma 17.** Any dyadic probability, ∑^j_i=1(1/2^a_i) with (a₁, ..., a_j) strictly increasing, can be constructed with O(k²) pins from a set of O(k) pure streams, with ordered probabilities from higher to lower and with O(1) distance between neighboring streams, if, for every a_i, there is a pure stream with probability 1/2^a_i.

**Proof.** Begin by adjusting the distances between the pure streams, using R-pins and L-pins so that neighboring streams have 3 empty columns between them. This takes O(k²) pins, since each pure stream can move O(k) units. We define the stream of probability 1/2^a_j as the working stream. We will bring it to the left using L-pins. If the pure stream immediately to the left of it has probability 1/2^a_i for some i and the next pure stream to the left has a greater probability, merge the 1/2^a_i stream with the working stream. Otherwise, use a crossover gadget to bring the working stream to the left. When there are no more pure streams to the left, the working stream will have probability ∑^j_i=1(1/2^a_i). Each crossover costs O(k), by Lemma 16, and the number of crossovers needed is O(k). Therefore, the number of required pins is O(k²). □

**Theorem 18.** Any dyadic probability distribution (p₁, p₂, ..., pₙ) can be obtained with O(nk²) pins, in which k is the maximum number of bits used to represent a probability in the desired distribution.

**Proof.** This is a proof by construction. Begin by splitting the initial stream (of probability 1) into k − 1 streams of probabilities (1/2^i)^(k−1)_i=1 ordered by decreasing order from left to right followed by two streams of probability 1/2^k. This can be done by successively splitting the rightmost stream with an N-pin, k times.

Using Lemma 17, we can obtain p₁ using O(k²) pins. Notice that this construction decreases the number of pure streams and that Lemma 17 cannot be applied directly to obtain p₂, because a term 1/2^m of p₂ might not have a corresponding pure stream. We then re-split the pure streams even further using the following algorithm.

Let 1/2^p be the smallest term of p₂ that does not have a corresponding pure stream, and let 1/2^q be the smallest pure stream such that p > q. Divide the stream 1/2^q with N-pins in the same way as the initial stream was split, p − q times, so that the resultant rightmost stream will have probability 1/2^p. This procedure maintains the order of probabilities. Repeat until all terms of p₂ have corresponding streams. This subroutine uses O(k) pins for each iteration. Overall, O(k²) pins are used, because p₂ has O(k) bits. After this re-splitting phase, the number of pure streams cannot be greater than 2k. If it were, there would be more than 2 streams with the same probability, because the smallest probability that can be created is 1/2^k. One of them would have been created in the re-splitting phase, which causes a contradiction since we only create probabilities that were previously inexistent.

The subroutine described above allows the construction of p₂ with O(k²) pins, by applying Lemma 17. All statements referring to p₂ can be generalized to p_i, i > 2. We can then construct all probabilities with a total of O(nk²) pins. □

## 5. Conclusion

In this paper we have analyzed a rich set of models of perfectly inelastic Pachinko with a single ball as input. A natural open question is to extend these results for Pachinko allowing multiple input balls. For Grid Pachinkos, we allow for pins that stop, move or evenly split the forward movement of a ball. Another natural generalization arises by allowing for (α, β)-pins, pins where the ball will either move to the right or left with probability α·β or (1 − α)·β respectively and continue moving down or remain at the pin with probability 1 − β. All our Grid Pachinko pins are special cases of this model. Modeling elastic, non-local behavior, where balls may move up and/or more to the left or right, is perhaps the more natural, yet more complex system to analyze. Additionally, our models consider Pachinko in a vertical plane. Modeling three-dimensional balls interacting with point or line obstacles in the presence of gravity may also lead to interesting future work.

### Acknowledgements

The authors would like to thank Zachary Abel, Greg Aloupis, Nadia Benbernou, Scott Kominers, and Anika Rounds for helpful discussions.

### References

1. Milton Abramowitz, Irene A. Stegun, et al., Handbook of Mathematical Functions, Applied Mathematics Series, vol. 55, 1966, p. 62.
2. Jin Akiyama, Mari-Jo P. Ruiz, Pachinko math, in: A Day's Adventure in Math Wonderland, World Scientific, 2008.
3. Nicolas Bonichon, Cyril Gavoille, Nicolas Hanusse, Dominique Poulalhon, Gilles Schaeffer, Planar graphs, via well-orderly maps and trees, Graphs Comb. 22 (2006) 185–202.
4. Mark De Berg, Marc Van Kreveld, Mark Overmars, and Otfried Cheong Schwarzkopf, Computational Geometry, Springer, 2000.
5. Eames Office. Mathematica: a world of numbers . . . and beyond, http://www.eamesoffice.com/the-work/mathematica/.
6. R.L. Graham, M. Grötschel, L. Lovász (Eds.), Handbook of Combinatorics, vol. 2, MIT Press, Cambridge, MA, USA, 1995.
7. JapanZone. Pachinko, http://www.japan-zone.com/modern/pachinko.shtml.
8. Wikipedia. Pachinko, http://en.wikipedia.org/wiki/Pinball.