# Decision Log

## Assumptions

* Each user has only one wallet
* A coupon can be redeemed only once
* System may receive duplicate requests due to network retries
* Users may try to redeem same coupon at the same time

---

## Edge Cases Handled

* Duplicate requests → handled using IdempotencyKey
* Coupon already redeemed → blocked
* Expired coupon → validation added
* Concurrency → only one request can redeem coupon
* Partial failure → handled using reconciliation API

---

## What Can Still Break

* Database downtime can stop the system
* High traffic without scaling may affect performance
* If reconciliation is not run, inconsistencies may remain
* Network failures may delay responses

---

## Trade-offs

* Chose simple DB-based locking instead of distributed locking
* Used balance column instead of wallet ledger for simplicity
* Did not implement caching to keep system simple
* Focused more on correctness than performance optimization
