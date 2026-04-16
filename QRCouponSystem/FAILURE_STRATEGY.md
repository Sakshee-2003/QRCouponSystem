# Failure Strategy

## Database Failure

If database is down:

* API will fail and return error
* Transaction will rollback automatically
* No partial data will be saved

---

## Duplicate Requests

If same request comes multiple times:

* IdempotencyKey is checked
* System returns previous result
* No duplicate wallet update happens

---

## Partial Failure

Example:

* Wallet updated but coupon not marked redeemed

Solution:

* Reconciliation API detects inconsistency
* Fixes coupon and wallet state

---

## Concurrency Issue

If multiple users try same coupon:

* Only one update succeeds
* Others fail safely

---

## Recovery Mechanism

* Reconciliation API fixes data issues
* Transaction logs help identify failures
