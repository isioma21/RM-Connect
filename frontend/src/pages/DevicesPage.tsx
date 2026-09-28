import { useEffect, useState } from 'react'
import { Box, Button, Card, Chip, CircularProgress, Stack, Typography } from '@mui/material'
import { sessionsApi } from '../api/sessions'
import type { Session } from '../api/types'
import { ConfirmDialog } from '../components/ConfirmDialog'
import { ErrorAlert } from '../components/ErrorAlert'
import { deviceName } from '../utils/device'
import { formatDay, formatTime } from '../utils/format'

export function DevicesPage() {
  const [sessions, setSessions] = useState<Session[]>()
  const [error, setError] = useState<unknown>(null)
  const [toSignOut, setToSignOut] = useState<Session | null>(null)
  const [confirmingOthers, setConfirmingOthers] = useState(false)

  function load() {
    sessionsApi.mine().then(setSessions).catch(setError)
  }

  useEffect(load, [])

  async function run(action: () => Promise<void>) {
    setToSignOut(null)
    setConfirmingOthers(false)
    setError(null)
    try {
      await action()
      load()
    } catch (err) {
      setError(err)
    }
  }

  if (!sessions) return error ? <ErrorAlert error={error} /> : <CircularProgress aria-label="Loading" />

  const others = sessions.filter((session) => !session.isCurrent)

  return (
    <Stack spacing={2.5} sx={{ maxWidth: 760 }}>
      <Stack direction={{ xs: 'column', sm: 'row' }} sx={{ justifyContent: 'space-between', alignItems: { sm: 'center' }, gap: 2 }}>
        <Box>
          <Typography variant="h1">Devices</Typography>
          <Typography color="text.secondary">Where you are signed in. Sign out any device you don't recognise.</Typography>
        </Box>
        {others.length > 0 && (
          <Button variant="outlined" color="error" onClick={() => setConfirmingOthers(true)}>
            Sign out all other devices
          </Button>
        )}
      </Stack>
      <ErrorAlert error={error} />

      <Card>
        {sessions.map((session, index) => (
          <Stack
            key={session.id}
            direction="row"
            sx={{ p: 2.5, gap: 2, alignItems: 'center', borderTop: index ? 1 : 0, borderColor: 'divider' }}
          >
            <Box sx={{ flexGrow: 1 }}>
              <Stack direction="row" sx={{ gap: 1, alignItems: 'center' }}>
                <Typography sx={{ fontWeight: 700 }}>{deviceName(session.device)}</Typography>
                {session.isCurrent && <Chip label="This device" size="small" color="primary" />}
              </Stack>
              <Typography color="text.secondary" variant="body2">
                IP {session.ipAddress} · signed in {formatDay(session.signedInAt)}, {formatTime(session.signedInAt)}
                {!session.isCurrent && ` · last active ${formatTime(session.lastSeenAt)}`}
              </Typography>
            </Box>
            {!session.isCurrent && (
              <Button variant="outlined" color="error" onClick={() => setToSignOut(session)}>Sign out</Button>
            )}
          </Stack>
        ))}
      </Card>

      <ConfirmDialog
        open={toSignOut !== null}
        title="Sign out device?"
        message={`Are you sure you want to sign out ${toSignOut ? deviceName(toSignOut.device) : ''}?`}
        confirmLabel="Yes, sign out"
        onConfirm={() => toSignOut && run(() => sessionsApi.signOut(toSignOut.id))}
        onClose={() => setToSignOut(null)}
      />
      <ConfirmDialog
        open={confirmingOthers}
        title="Sign out other devices?"
        message={`Are you sure you want to sign out ${others.length} other ${others.length === 1 ? 'device' : 'devices'}?`}
        confirmLabel="Yes, sign out"
        onConfirm={() => run(sessionsApi.signOutOthers)}
        onClose={() => setConfirmingOthers(false)}
      />
    </Stack>
  )
}
