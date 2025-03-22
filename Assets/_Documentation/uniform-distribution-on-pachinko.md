# Uniform Distribution On Pachinko

**Naoki Kitamura**  
Nagoya Institute of Technology, Syowa-ku, Gokiso-cho, Nagoya, Aichi, 466-8555, Japan  
29414045@stn.nitech.ac.jp

**Yuya Kawabata**  
Nagoya Institute of Technology, Syowa-ku, Gokiso-cho, Nagoya, Aichi, 466-8555, Japan  
29414043@stn.nitech.ac.jp

**Taisuke Izumi**  
Nagoya Institute of Technology, Syowa-ku, Gokiso-cho, Nagoya, Aichi, 466-8555, Japan  
t-izumi@nitech.ac.jp

## Abstract

Pachinko is a Japanese mechanical gambling game similar to pinball. Recently, Akitaya et al. proposed several mathematical models of Pachinko. A number of pins are spiked in a field. A ball drops from the top-side end of the playfield, and falls down. In the 50-50 model, if the ball hits a pin, it moves to the left or right of the pin with equal probability. An arrangement of pins generates a distribution of the drop probability over all columns. We consider the problem of generating uniform distributions. Akitaya et al. show that (1/2^a)-uniform distribution is possible for a ∈ {0, 1, 2, 3, 4} and conjectured that it is possible for any positive integer a. In this paper, we show that the conjecture is true by a constructive way.

**2012 ACM Subject Classification:** Mathematics of computing → Combinatoric problems

**Keywords and phrases:** Pachinko, discrete mathematics

**Digital Object Identifier:** 10.4230/LIPIcs.FUN.2018.26

## 1 Introduction

### 1.1 Background

Pachinko is a Japanese mechanical gambling game similar to pinball (Figure 1). The machine stands up vertically, and the player shoots a metal ball into the playfield. Many pins are spiked in the playfield, and the ball drops from the top of the field. If it goes into a pocket in the field, then the player gets some reward. Recently, Pachinko is analyzed in the context of discrete mathematics. The origin of mathematical Pachinko is the book written by Akiyama in 2008 [3], and recently, Akitaya et al. study an idealized geometry of a simple form of Pachinko [2]. In this paper, we consider one of the mathematical models, called 50-50 model, posed there.

The 50-50 model consists of three factors, field, pins, and a ball. The field is a half-plane triangle lattice with the top-side end. We can put a pin at any lattice point. A row is a horizontal line where lattice points exist, and a column is a vertical line where lattice points exist. Since we consider the triangle lattice, intersection points of rows and columns do not necessarily have a lattice point (see Figure 2). The ball drops from the center of the top end and falls down vertically. If the ball hits a pin, then it moves to the left or right of the pin with equal probability, and the ball continues to fall down vertically. Once we fix a pin arrangement under the 50-50 model, we can calculate the probability that the ball drops to each column. Then we can define its inverse problem of "deciding whether there exists a pin arrangement generating a given distribution or not".

### 1.2 Problem and Our Result

In [2], it is shown that any probability distribution ⟨p₁, p₂, ..., pₙ⟩ in the 50-50 model can be constructed within an arbitrarily small additive error, and thus the main interest is the exact generation of a given distribution. The (1/2^a)-uniform distribution in the 50-50 model is the probability distribution, where the probability that the ball drops at the center is 0 and the probability at the 2^a closest coordinates from the center is 1/2^a (see Figure 3). Akitaya et al. show that the (1/2^a)-uniform distribution for a ∈ {0, 1, 2, 3, 4} can be constructed, and they conjecture that 2^a uniform distribution for any positive integer a can be constructed in [2]. The contribution of this paper is to show that this conjecture is true. That is, for any a ≥ 1, the (1/2^a)-uniform distribution can be constructed. Moreover, the construction can be done using only a polynomial of 2^a number of pins. To show the result, we introduce a new language-theoretic formulation, which is simple but substantially useful for the analysis of the 50-50 model.

## 2 Preliminaries

### 2.1 Configuration and Rewriting Rule

We formulate the problem in the 50-50 model using the notion of formal grammar. A Pachinko machine is represented by a triangle lattice on a half plane with infinite horizontal length and infinite downward vertical length. Each horizontal line containing lattice points is called a row. From the top-side end, we assign each row with a y-coordinate 1, 2, . . . . Since the field is a triangle lattice, the lattice points on an odd row are half-shifted from those on an even row. To fit them into the standard orthogonal coordinate system, we assign even x-coordinates to the lattice points in even rows, and odd x-coordinates to those in odd rows (see Figure 4). Any coordinate (i, j) ∈ N × N₊ for i and j with different parity is not a lattice point, which is the space for the ball to drop down to lower rows. Those coordinates are called passages. Initially, the ball is dropped from the horizontal center of the top-side. Hence, the probability that the ball passes through (0, 0) is one. A pin can be placed at any lattice point. In the 50-50 model, if the dropping ball hits a pin at point (i, j) (i.e., passes through (i, j − 1)), it moves to either (i − 1, j) or (i + 1, j) with probability 1/2. If no pin is spiked at (i, j), the drop probability of (i, j) is equal to that of (i, j − 1).

A pin arrangement is a set of lattice points where a pin is spiked. Given a pin arrangement P and any i ≥ 1, P generates the drop probability distribution over all coordinates in the i-th row, which is called the i-th configuration of P (or simply say a configuration). Formally, a configuration is a finite odd-length sequence of rational values whose sum is equal to one, where the center of the sequence corresponds to the drop probability at x-coordinate zero and two infinite sequences of zeros spanning x-coordinates ±∞ are cut off. Throughout this paper, we assume the minimum granularity 1/2^g of each probability for some g ≥ 1. Then, by multiplying each value by 2^g, we can treat any configuration as a sequence of non-negative integer values.

The change of configurations (i.e., the change of the corresponding probability distribution) by placing a pin at a lattice point is expressed as an application of rewriting rules in formal grammar. While we can put two or more pins in the same row, such a pin placement is equivalently translated into the placement in a number of rows where each row contains exactly one pin. Thus, without loss of generality, we assume that each row contains one pin. We regard each configuration as a word over the symbol set [0, 2^g]. If a pin is put at a lattice point with x-coordinate (i, j + 1), the probability mass of coordinate (i, j) is evenly split into (i − 1, j + 1) and (i + 1, j + 1), which is expressed by the rewriting rule as follows:

**Definition 1.**
abc → [a + b/2]0[c + b/2] (Rule R1).

where bracket [] represents the single symbol corresponding to the value inside. The symbol a or c may be an implicit zero value omitted in the representation of configurations. An example of rewriting is illustrated in Figure 4.

### 2.2 Symmetric Configuration

Throughout this paper, we only consider symmetric configurations, that is, the configurations mirror-symmetric about the center. To express symmetric configurations, the right side from the center is not necessary. More precisely, we denote a symmetric configuration w[v]w^R as w[v/2]$, where w^R is the inversion of w, and $ is the special symbol representing the right side from the center (say the boundary). Except for the center, any rewriting is applied symmetrically. That is, in the transformation of symmetric configurations, putting a pin at (i, j) implies putting another pin at (−i, j). The exceptional case is the rewriting at the center, which is handled by the special rule below (note that the drop probability at the center is expressed by its half).

**Definition 2.**
ab$ → [a + b]0$ (Rule R2).

The symbol $ corresponds to the center. Figure 5 is an example of how symmetric configurations are rewritten by Rule R2. If we can generate a configuration u$ from w$ by finite-time applications of rewriting rules, then we say u$ is transformed from w$, and write w$ ⇒ u$. We also extend this notion of transformability into substring cases. Let xyz$ and xy′z$ be the two words such that y and y′ have the same length. If xyz$ ⇒ xy′z$ holds for any x and z, we say y′ is transformed from y, and write y ⇒ y′.

### 2.3 Formulation of the Problem

In this section we formalize the problem of generating uniform distributions. The goal of the problem is to generate the probability distribution of 1/2^a, 1/2^a, · · · ,1/2^a, 0, 1/2^a, 1/2^a, · · · , 1/2^a. We call it the (1/2^a)-uniform distribution. In our construction, the minimum granularity 1/2^(a+1) of drop probability suffices to generate the (1/2^a)-uniform distribution, and thus the problem is reduced to the transformability of [2^(a+1)]$ ⇒ 2^(2^a)$ (upper subscripts mean repetition of symbols). The problem actually we solve is a "recursive" version of this transformability, which is stated by the following Theorem.

**Theorem 3.** 4^k 0$ ⇒ 2^(2k) 0$ holds for a ≥ 4 and k = 2^a.

In [2], it has been proved that (1/2^a)-uniform distribution can be generated for a ≤ 4. By applying Theorem 3 iteratively, we can show that the (1/2^a)-uniform distribution can be generated for any a ≥ 1.

## 3 Generating Uniform Distribution

The whole of Section 3 is devoted to the proof of Theorem 3. The proof consists of the following three parts.

1. 4^k 0$ ⇒ (440)^(k/2) $.
2. (440)^(k/2) $ ⇒ 42^(k−3) 02^(k−1) 4$.
3. 42^(k−3) 02^(k+1) 4$ ⇒ 2^(2k) 0$.

Clearly, the combination of these transformations results in Theorem 3. In the following subsections, we look at the details of each part.

### 3.1 Part 1: From 4^k 0$ to (440)^(k/2) $

First, we explain a preliminary lemma.

**Lemma 4.** Let x, y, and z be any symbols, and j be a positive integer greater than or equal to 3. Then the following transformations are possible.

xy^j z ⇒ [x + y]0y^(j−2)0[z + y]. (1)

xy^j 0$ ⇒ [x + y]0y^(j−1)0$. (2)

**Proof.** We first consider the transformation (1). The proof is based on the induction on j.

(Basis) In the case of j = 3, we have the following transformation (each underline represents the position of rewriting):

xyyyz ⇒ [x + y/2]0[2y]0[z + y/2] ⇒ [x + y/2]y0y[z + y/2] ⇒ [x + y]0y0[z + y].

(Inductive step) Suppose as the induction hypothesis that the transformation (1) is possible for j = k. The case of j = k + 1 is obtained as follows:

xy^(k+1)z = xy^kyz ⇒ (Induction hypothesis) [x + y]0y^(k−2)0[2y]z ⇒ [x + y]0y^(k−2)y0[y + z] = [x + y]0y^(k−1)0[y + z].

Thus the transformation (1) is possible. The proof of the transformation (2) follows the rewriting process below:

xy^j 0$ ⇒ (Transformation (1)) [x + y]0y^(j−2)0y$ ⇒ [x + y]0y^(j−2)y0$ = [x + y]0y^(j−1)0$.

The lemma is proved. □

For simplicity of arguments, we pad an appropriate number of zeros to the left side of w such that the number of zeros in w becomes exactly k/2 + 1. The i-th run of w (1 ≤ i ≤ k/2) is the substring between i-th zero and (i + 1)-th zero (indexed from the left end of w). The length of the i-th run in w is denoted by l_w(i). Now we define the notion of Normal Forms (NFs), which is the class of configurations we have to treat in the proof of Part 1.

**Definition 5.** A word w is a normal form(NF) with respect to k if and only if every run in w consists of only symbol 4, the number of runs (of 4) is at most k/2, and the symbol neighboring to the boundary is 0.

Let l_w(j) be the length of j-th run in NF w. The run-length vector v(w) of w is the k/2-dimensional vector whose j-th element corresponds to l_w(j). Let vol_w(h) = ∑_(j∈[1,h]) l_w(j). Then SNFs are defined as follows:

**Definition 6.** A NF w (with respect to k) is a strongly-normal form(SNF) (with respect to k) if and only if it satisfies vol_w(h) ≤ 2h for any h ∈ [1, k/2].

Note that 4^k 0$ and (440)^(k/2) $ are both SNFs. For any two SNFs w₁ and w₂, we define c(w₁, w₂) to be the minimum index such that l_w₁(c(w₁, w₂)) ≠ l_w₂(c(w₁, w₂)) holds, and define N_k as the set of all SNFs with respect to k. Then we define a total order ≺ over N_k by the lexicographic order of corresponding run-length vectors. That is, we define

w₁ ≺ w₂ ⇔ l_w₁(c(w₁, w₂)) ≤ l_w₂(c(w₁, w₂)).

For any SNF w, let t(w) be the position of the leftmost run with length more than two, that is, t(w) = min_(j∈[1,k/2],l_w(j)≥3) j. If no run has a length more than two, we define t(w) = k/2 + 1. The rewriting process of 4^k 0$ ⇒ (440)^(k/2) $ is to iterate the application of Lemma 4 (1) (if t(w) < k/2) or (2) (if t(w) = k/2) to the t(w)-th run, until the transformation reaches the word w′ with t(w) = k/2 + 1. In the remaining part of this section we show that this process correctly creates (440)^(k/2).

**Lemma 7.** Let x be any SNF, and x′ be the word after the application of Lemma 4 to the t(x)-th run in x. Then, x′ is also an SNF and x ≺ x′.

**Proof.** It is easy to check that any run of x′ consists of only 4s and symbol 0 is the neighbor of $ in x′. By the definition of SNFs for h = 1, for any SNF w, l_w(1) ≤ 2 holds and thus t(w) > 1 necessarily holds. Since we have to apply Lemma 4 to the first run for increasing the number of runs to more than k/2, the number of runs in x′ is at most k/2. Consequently x′ is a NF. Since the application of Lemma 4 at the i-th run of a word w increases l_(i−1)(w) and l_(i+1)(w) by one, and decreases l_i(w) by two, the transformation from x to x′ can increase only the value of vol_x(t(x) − 1). For showing that x′ is an SNF, it suffices to prove vol_x′(t(x) − 1) ≤ 2(t(x) − 1). By the fact of l_x(t(x)) ≥ 3, we have vol_x(t(x) − 1) + 3 ≤ vol_x(t(x)) ≤ 2t(x) and thus vol_x(t(x) − 1) ≤ 2(t(x) − 1) − 1 holds. Since the length of t(x)-th run increases at most by one after the application of Lemma 4. We obtain vol_x′(t(x) − 1) ≤ vol_x(t(x) − 1) + 1 ≤ 2(t(x) − 1). Thus x′ is a SNF. By the definition, c(w, w′) = t(x) − 1 holds and thus we obtain l_(c(w,w′))(w′) > l_(c(w,w′))(w), that is x′ ≺ x. The lemma is proved □

**Lemma 8.** The word (440)^(k/2) $ is the maximum element with respect to ≺.

**Proof.** Let w = (440)^(k/2) $. Suppose for contradiction that a SNF w′ satisfies w ≠ w′ and w ≺ w′. Then, vol_w(c(w, w′)) < vol_w′(c(w, w′)) holds. However, since vol_w(c(w, w′)) = 2c(w, w′) holds, we have vol_w′(c(w, w′)) > 2c(w, w′). It contradicts the fact that w′ is an SNF. □

The two lemmas above imply that our rewriting process eventually leads the maximum element of SNFs, and thus the following corollary holds.

**Corollary 9.** Let k ∈ N be any even positive integer. Then the following transformation is possible.

4^k 0$ ⇒ (440)^(k/2) $.

### 3.2 Part 2: From (440)^(k/2) $ to 42^(k−3) 02^(k−1) 4$

In this section, we first introduce a magical string B_i = 42^i 02^(i+1) 4$, as well as its nice properties. Before showing the properties of B_i, we present further preliminary lemmas.

**Lemma 10.** Let x,y, and z be any symbols, and j be any positive integer. Then the following transformations are possible.

x[2y]y^j z ⇒ [x + y]y^(j−1)0[2y]z. (3)

x[2y]y^j z ⇒ [x + y]y^j 0[z + y]. (4)

**Proof.** We first consider the transformation (3). The proof is based on the induction on j.

(Basis) In the case of j = 1, we can have the following transformation:

x[2y]yz ⇒ [x + y]0[2y]z.

(Inductive step) Suppose as the induction hypothesis that the transformation (3) is possible for j = k. The case of j = k + 1 is obtained as follows:

x[2y]y^(k+1)z ⇒ [x + y]0[2y]y^k z ⇒ (Induction hypothesis) [x + y]y^k 0[2y]z.

The proof for the transformation (4) follows the rewriting process presented below:

x[2y]y^j z ⇒ (Transformation(3)) [x + y]y^(j−1)0[2y]z ⇒ [x + y]y^j 0[z + y].

The lemma is proved. □

**Corollary 11.** Let x,y, and z be any symbols, and j be any positive integer. Then the following transformations are possible.

xy^j[2y]z ⇒ x[2y]0y^(j−1)[z + y]. (5)

xy^j[2y]z ⇒ [x + y]0y^j[z + y]. (6)

**Lemma 12.** Let j be a positive integer greater than or equal to 4. Then 02^j 4$ ⇒ 2^2 02^(j−2) 4$ holds.

**Proof.** We can rewrite 02^j 4$ as follows.

02^j 4$ = 022222^(j−4) 4$ ⇒ (Lemma 4 (1), x = 0, y = 2, z = 2) 202042^(j−4) 4$ ⇒ (Lemma 10 (4), x = 0, y = 2, z = 2) 202^(j−3) 044$ ⇒ 202^(j−3) 080$ ⇒ 202^(j−3) 404$ ⇒ (Corollary 11 (6), x = 0, y = 2, z = 0) 2202^(j−3) 24$ = 2^2 02^(j−2) 4$.

The lemma is proved. □

The goal of Part 2 is to obtain B_(k−3) from (440)^(k/2). We introduce two important properties of B_i = 42^i 02^(i+2) 4$, which is the primary reason why we claim that B_i is "magical".

**Lemma 13.** Let i be any positive integer. Then the following transformations are possible.

04B_i ⇒ 40B_i. (7)

0440B_i ⇒ B_(i+2). (8)

**Proof.** We first consider the transformation (7), which is obtained as follows.

04B_i = 0442^i 02^(i+2) 4$ ⇒ 06042^(i−1) 02^(i+2) 4$ ⇒ (Lemma 10 (4), x = 0, y = 2, z = 0) 062^i 02^(i+3) 4$ ⇒ (Lemma 12) 062^(i+2) 02^(i+1) 4$ ⇒ (Lemma 4 (1), x = 6, y = 2, z = 0) 0802^i 02^(i+2) 4$ ⇒ 4042^i 02^(i+2) 4$ = 40B_i.

The proof for the transformation (8) follows the rewriting process below:

0440B_i = 044042^i 02^(i+2) 4$ ⇒ 0442^(i+1) 02^(i+3) 4$ = 04B_(i+1) ⇒ (Transformation (7)) 40B_(i+1) = 4042^(i+1) 02^(i+3) 4$ ⇒ (Lemma 10 (4), x = 0, y = 2, z = 0) 42^(i+2) 02^(i+4) 4$ = B_(i+2).

The lemma is proved. □

Why these properties are so important? The intuitive understanding of the reason for the first property is that we can treat B_i as $. In Part 1, we only use the application of rule R2 for b = 4. Then the behaviors of 4$ and 4B_i are the same, and thus any transformation in Section 3.1 applicable to w′$ is also applicable to w′B_i. This fact yields the corollary below.

**Corollary 14.** Let k′ ∈ N be an even positive integer, and w be a SNF with respect to k′. Letting w′ be the word obtained from w by deleting $, wB_i ⇒ (440)^(k′/2) B_i holds.

Combining this corollary with the second property of Lemma 13, we can show that B_i can recursively "absorb" substring 440 to make itself grow up. The following lemma corresponds to the base case of this rewriting process.

**Lemma 15.**
(440)^4 $ ⇒ 44440B_1.

**Proof.** Deferred to the appendix. □

The following two lemmas are the main body of Part 2, which shows the rewriting process of absorbing substring 440.

**Lemma 16.** Let i and j be any positive integer. Then 0^i (440)^i B_j ⇒ B_(j+2i) holds.

**Proof.** The proof is based on the induction on i. (Basis) In the case of i = 1, we have the following transformation:

0440B_j ⇒ (Lemma 13 (8)) B_(j+2),

and in the case of i = 2, we also have the following transformation:

0^2 (440)^2 B_j = 0^2 440440B_j ⇒ (Lemma 13 (8)) 0^2 44B_(j+2) ⇒ (Lemma 13 (7)) 0^2 80B_(j+2) ⇒ 0404B_(j+2) ⇒ (Lemma 13 (7)) 0440B_(j+2) ⇒ (Lemma 13 (8)) B_(j+4).

(Inductive step) Suppose as the induction hypothesis that Lemma 16 holds for i = k (k ≥ 2). The case of i = k + 1 is proved by:

0^(k+1) (440)^(k+1) B_j (Because of k ≥ 2) = 0^(k+1) (440)^(k−2) 440440440B_j ⇒ (Lemma 13 (8)) 0^(k+1) (440)^(k−2) 44044B_(j+2) ⇒ (Lemma 13 (7)) 0^(k+1) (440)^(k−2) 44080B_(j+2) ⇒ 0^(k+1) (440)^(k−2) 44404B_(j+2) ⇒ (Lemma 13 (7)) 0^(k+1) (440)^(k−2) 44440B_(j+2) ⇒ (Corollary 14) 0^k (440)^k B_(j+2) ⇒ ( Induction hypothesis) B_(j+2(k+1)).

The Lemma is proved. □

**Lemma 17.** Let k be a positive integer greater than or equal to 8. Then (440)^(k/2) $ ⇒ B_(k−3) holds.

**Proof.**
(440)^(k/2) $ = (440)^(k/2−4) (440)^4 $ ⇒ (Lemma 15) (440)^(k/2−4) 44440B_1.

(440)^(k/2−4) 44440 is an SNF (with respect to k − 2). Thus, we can rewrite it as follows.

(440)^(k/2−4) 44440B_1 ⇒ (Corollary 14) (440)^(k/2−2) B_1 ⇒ (Lemma 16) B_(k−3).

The lemma is proved. □

### 3.3 Part 3: From 42^(k−3) 02^(k−1) 4$ to 2^(2k) 0$

Finally, we prove that 42^(k−3) 02^(k−1) 4$ can be transformed into 2^(2k) 0$. We explain four preliminary lemmas used in this section.

**Lemma 18.** Let x, z be any positive integers, and y be a positive integer greater than or equal to 2. Then the following transformation is possible.

02^x 02^(2y) 02^z $ ⇒ 02^(x+y−1) 02^2 02^(z+y−1) $.

**Proof.** The proof is based on the induction on y. (Basis) In the case of y = 2, we can have the following transformation:

02^x 02^4 02^z $ ⇒ (Lemma 4 (1), x = 0, y = 2, z = 0) 02^(x+1) 02^2 02^(z+1) $.

(Inductive step) Suppose as the induction hypothesis that the Lemma 18 holds for y = k. The case of y = k + 1 is proved by:

02^x 02^(2(k+1)) 02^z $ ⇒ (Lemma 4 (1), x = 0, y = 2, z = 0) 02^(x+1) 02^(2k) 02^(z+1) $ ⇒ (Induction hypothesis) 02^(x+k) 02^2 02^(z+k) $.

The Lemma is proved. □

**Lemma 19.** Let i be a positive integer greater than or equal to 5. Then 02^2 02^i 4$ ⇒ 2^(i−4) 02^2 02^4 4$ holds.

**Proof.** The proof is based on the induction i. (Basis) In the case of i = 5, we can have the following transformation:

02^2 02^5 4$ ⇒ (Lemma 12) 02^2 2^2 02^3 4$ ⇒ (Lemma 4 (1), x = 0, y = 2, z = 0) 202^2 02^4 4$.

(Inductive step) Suppose as the induction hypothesis that Lemma 19 holds for i = k. The case of i = k + 1 is proved by:

02^2 02^(k+1) 4$ ⇒ (Lemma 12) 02^2 2^2 02^(k−1) 4$ ⇒ (Lemma 4 (1), x = 0, y = 2, z = 0) 202^2 02^k 4$ ⇒ (Induction hypothesis) 2·2^(k−3) 02^2 02^4 4$ = 2^(k−3) 02^2 02^4 4$.

The lemma is proved. □

**Lemma 20.** Let i be any positive integer. Then x2^i $ ⇒ [x + 2]2^(i−1) 0$ holds.

**Proof.** The proof is based on the induction on i. (Basis) In the case of i = 1, we have the following transformation:

x2$ ⇒ [x + 2]0$.

(Inductive step) Suppose as the induction hypothesis that Lemma 20 holds for i = k. For the case of i = k + 1, we have the transformation as follows:

x2^(k+1) $ = x22^k $ ⇒ (Induction hypothesis) x42^(i−2) 0$ ⇒ (Lemma 10 (4), x = x, y = 2, z = 0) [x + 2]2^(i−2) 02$ ⇒ [x + 2]2^(i−1) 0$.

The case of i = k + 1 is proved, and thus the lemma holds. □

**Lemma 21.**
022022224$ ⇒ 222222220$.

**Proof.** Deferred to the appendix. □

The combination of the four lemmas straightforwardly deduces the main lemma of Part 3.

**Lemma 22.** Let k be a positive integer greater than or equal to 8. The following transformation is possible.

0042^(k−3) 02^(k−1) 4$ ⇒ 2^(2k) 0$.

**Proof.** We can have the following transformation:

0042^(k−3) 02^(k−1) 4$ ⇒ (Lemma 10 (4), x = 0, y = 2, z = 0) 02^(k−2) 02^k 4$ ⇒ (Lemma 18) 2^(k/2−2) 02^2 02^(3k/2−2) 4$ ⇒ (Lemma 19) 2^(k/2−2) 2^(3k/2−6) 02^2 02^4 4$ = 2^(2k−8) 02^2 02^4 4$ ⇒ (Lemma 21) 2^(2k−8) 2^8 0$ = 2^(2k) 0$.

□

## 4 Conclusions and discussion

In this paper, we proved that (1/2^a)-uniform distributions in the 50-50 model can be generated for any a ≥ 1. This is the complete positive answer for the open problem posed by [2]. In this article we do not consider the complexity of the generation process — the number of pins, or the number of rows. While it is not difficult to bound the number of pins used in our construction by a polynomial of 2^a, its fine-grained analysis is not proposed yet (following a rough estimation it is bounded by O(2^(4a)), but the tight analysis is probably O(2^(3a)) pins).

The complexity on the number of rows is much complicated. In our construction, the restriction of one pin at one row made the analysis so simple, but when we want to optimize the number of rows, that restriction cannot be used. It is also an interesting to reveal the computational complexity on the problem of generating given distributions. In the context of formal language theory, our rewriting rule is not a context-free grammar, and thus it is not clear if the decision problem on the generability of a given distribution is in class P or not.

## References

1. Pachinko - wikipedia. URL: https://en.wikipedia.org/wiki/Pachinko.
2. Hugo A Akitaya, Erik D Demaine, Martin L Demaine, Adam Hesterberg, Ferran Hurtado, Jason S Ku, and Jayson Lynch. Pachinko. Computational Geometry: Theory and Applications, 68, 2018.
3. Jin Akiyama and Mari-Jo P. Ruiz. Pachinko math.In A Day's Adventure in Math Wonderland. World Scientific, 2008.

## Omitted Proofs

**Lemma 15.**
(440)^4 $ ⇒ 44440B_1.

**Proof.** The lemma is proved by the following transformation:

(440)^4 $ = (440)^2 440440$ ⇒ (440)^2 602602$ ⇒ (440)^2 602620$ ⇒ (440)^2 610801$ ⇒ (440)^2 614041$ ⇒ (440)^2 630403$ ⇒ (440)^2 632023$ ⇒ (440)^2 640204$ ⇒ (440)^2 802204$ = 440440802204$ ⇒ 440444042204$ ⇒ (Lemma 10 (4), x = 0, y = 2, z = 0) 440444222024$ ⇒ (Lemma 4 (1), x = 4, y = 2, z = 0) 440446020224$ ⇒ 440608020224$ ⇒ 440640420224$ ⇒ 440804040224$ ⇒ 444044202224$ = 44404B_1 ⇒ (Lemma 13 (7)) 44440B_1.

□

**Lemma 21.**
022022224$ ⇒ 222222220$.

**Proof.** The lemma is proved by the following transformation:

022022224$ ⇒ (Lemma 4 (1), x = 0, y = 2, z = 2) 022202044$ ⇒ 022202080$ ⇒ 022202404$ ⇒ 022202440$ ⇒ 022202602$ ⇒ 022202620$ ⇒ 022210801$ ⇒ 022214041$ ⇒ 022230403$ ⇒ 022232023$ ⇒ 022240204$ ⇒ (Corollary 11 (6), x = 0, y = 2, z = 0) 202222204$ ⇒ 202222240$ ⇒ (Corollary 11 (6), x = 0, y = 2, z = 0) 220222222$ ⇒ (Lemma 20) 222222220$.

□