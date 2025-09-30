# /update Command – Planning Questions (Easy Checklist)

You can answer by checking boxes [x] and adding short notes. If you’re fine with the suggested defaults, just write: Use defaults.

---

## 1) Command name and where it runs

- [x] Name the new command: `/update` (default)
- [x] Run from VS Code command AND PowerShell (default)
- [x] Edit files in `specs/master` (allow SPECIFY_FEATURE override) (default)

Notes:

---

## 2) Which files can it change?

- [ ] Only `spec.md`
- [x] `spec.md` + also allow `research.md`, `data-model.md`, `contracts/*`, `quickstart.md` (default)
- [x] After an update, add a short note to `research.md` (Decision Record) (default)

Notes:

---

## 3) How do you want to give the new text?

- [ ] Replace a whole section (by section title) (default)
- [ ] Append text to a section (default)
- [ ] Insert text right after a heading (default)
- [ ] Replace the whole file (e.g., `spec.md`) with a new file (default)
- [ ] If no options given, open the file for editing (interactive) (default)

Notes:
Implement what you feel is best
---

## 4) If the section name isn’t found

- [x] Create the section at the end (default)
- [ ] Stop with an error

Notes:

---

## 5) Check the spec after changes

- [x] Run validation automatically after update (default)

- If validation fails:
  - [ ] Undo the change (auto-rollback) (default)
  - [ ] Keep the change and show errors
  - [x] Show user what changes must me made to make validation successful, including specific line numbers and suggestions.

Notes:

---

## 6) Team safety (locks)

- [ ] Require lock; fail if someone else holds it (default)
- [x] Allow `--force` to bypass lock (default is allowed)

Notes:

---

## 7) Backups and history

- [x] Save a backup before changing (default)
- [x] Add a Decision Record entry in `research.md` with a message you provide (default)

Notes:

---

## 8) What should the command print out?

- [x] Simple success/fail message (default)
- [ ] Also a small JSON summary (file changed, operation, section, validation result) (default)

Notes:

---

## 9) Speed and safety

- [ ] Local file edits only (no internet) (default)
- [x] Aim to finish in a few seconds (default)
Notes:

---

## 10) Tests to add

- [x] Good cases and bad cases (e.g., missing section) (default)
- [x] Windows/macOS/Linux parity (default)

Notes:

---

## 11) Copilot Chat button

- [x] Add “GSC: Update” command in VS Code that shows a preview of the changes (default)

Notes:

---

## 12) Examples (pick what you like)

Not exactly sure what those examples should meant but here is what I am thinking.

Example:

/update master add new-feature {description: "Add a new feature"}
/update master remove new-feature {description: "Remove the new feature"}
/update master change new-feature {description: "Update the new feature"}
/update master revalidate new-feature, full spec, {user input} {description: "Revalidate the new feature"}

Notes:
