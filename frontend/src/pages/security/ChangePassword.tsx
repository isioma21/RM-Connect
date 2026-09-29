import { useState, type FormEvent } from 'react'
import {
  Box, Button, Card, CardContent, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Stack, Typography,
} from '@mui/material'
import { useNavigate } from 'react-router-dom'
import { authApi } from '../../api/auth'
import { useAuth } from '../../auth/AuthContext'
import { ErrorAlert } from '../../components/ErrorAlert'
import { PasswordField } from '../../components/PasswordField'

const emptyForm = { currentPassword: '', newPassword: '', confirmPassword: '' }

export function ChangePassword() {
  const { logout } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState(emptyForm)
  const [error, setError] = useState<unknown>(null)
  const [changed, setChanged] = useState(false)
  const [submitting, setSubmitting] = useState(false)

  const mismatch = form.confirmPassword !== '' && form.confirmPassword !== form.newPassword

  function update(field: keyof typeof emptyForm, value: string) {
    setForm({ ...form, [field]: value })
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    if (mismatch) return
    setSubmitting(true)
    setError(null)
    try {
      await authApi.changePassword({ currentPassword: form.currentPassword, newPassword: form.newPassword })
      setChanged(true)
    } catch (err) {
      setError(err)
    } finally {
      setSubmitting(false)
    }
  }

  // The API has already signed out every device; clear this one and go to login
  async function logInAgain() {
    await logout()
    navigate('/login')
  }

  return (
    <Stack spacing={2}>
      <Typography variant="h2">Change password</Typography>
      <Card>
        <CardContent component="form" onSubmit={handleSubmit} sx={{ p: 3 }}>
          <Stack spacing={2.5}>
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

      <Dialog open={changed} maxWidth="xs" fullWidth>
        <DialogTitle sx={{ fontFamily: "'Fraunces', Georgia, serif", fontWeight: 600 }}>Password changed</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Your password has been changed successfully. You have been logged out of all devices. Log in with your new password.
          </DialogContentText>
        </DialogContent>
        <DialogActions sx={{ px: 3, pb: 2.5 }}>
          <Button onClick={logInAgain} variant="contained">Log in again</Button>
        </DialogActions>
      </Dialog>
    </Stack>
  )
}
