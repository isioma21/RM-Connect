import { useState, type FormEvent } from 'react'
import { Alert, Box, Button, Card, CardContent, Stack, Typography } from '@mui/material'
import { authApi } from '../../api/auth'
import { ErrorAlert } from '../../components/ErrorAlert'
import { PasswordField } from '../../components/PasswordField'

const emptyForm = { currentPassword: '', newPassword: '', confirmPassword: '' }

export function ChangePassword({ onChanged }: { onChanged: () => void }) {
  const [form, setForm] = useState(emptyForm)
  const [error, setError] = useState<unknown>(null)
  const [changed, setChanged] = useState(false)
  const [submitting, setSubmitting] = useState(false)

  const mismatch = form.confirmPassword !== '' && form.confirmPassword !== form.newPassword

  function update(field: keyof typeof emptyForm, value: string) {
    setForm({ ...form, [field]: value })
    setChanged(false)
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    if (mismatch) return
    setSubmitting(true)
    setError(null)
    try {
      await authApi.changePassword({ currentPassword: form.currentPassword, newPassword: form.newPassword })
      setForm(emptyForm)
      setChanged(true)
      onChanged()
    } catch (err) {
      setError(err)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <Stack spacing={2}>
      <Typography variant="h2">Change password</Typography>
      <Card>
        <CardContent component="form" onSubmit={handleSubmit} sx={{ p: 3 }}>
          <Stack spacing={2.5}>
            {changed && <Alert severity="success">Password changed. Your other devices have been signed out.</Alert>}
            <ErrorAlert error={error} />
            <PasswordField
              label="Current password"
              value={form.currentPassword}
              onChange={(e) => update('currentPassword', e.target.value)}
              autoComplete="current-password"
              required
            />
            <PasswordField
              label="New password"
              value={form.newPassword}
              onChange={(e) => update('newPassword', e.target.value)}
              autoComplete="new-password"
              helperText="At least 8 characters, with upper and lower case letters and a number."
              required
            />
            <PasswordField
              label="Confirm new password"
              value={form.confirmPassword}
              onChange={(e) => update('confirmPassword', e.target.value)}
              autoComplete="new-password"
              error={mismatch}
              helperText={mismatch ? "Passwords don't match." : ' '}
              required
            />
            <Box>
              <Button type="submit" variant="contained" disabled={submitting || mismatch}>
                {submitting ? 'Changing…' : 'Change password'}
              </Button>
            </Box>
          </Stack>
        </CardContent>
      </Card>
    </Stack>
  )
}
